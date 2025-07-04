using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace UI.BotEditor {
    public class BotPreview : MonoBehaviour {
        [SerializeField]
        Button _zoomInButton;
        [SerializeField]
        Button _zoomOutButton;
        [SerializeField]
        Slider _zoomSLider;
        [SerializeField]
        Image _previewImage;
        [SerializeField]
        Image _maskImage;

        Sprite _defautlSprite;

        private bool _doCapture = false;
        private Texture2D _shotTexture = null;
        private Rect _shotRect;
        private Rect _overlapRect;
        // Start is called before the first frame update
        void Start() {
            _zoomInButton.onClick.AddListener(OnZoomInButtonClicked);
            _zoomOutButton.onClick.AddListener(OnZoomOutButtonClicked);
            _zoomSLider.onValueChanged.AddListener(OnZoomSliderValueChanged);
            _defautlSprite = _previewImage.sprite;
            _zoomSLider.minValue = 0.1f;
            _zoomSLider.maxValue = 2f;
            _zoomSLider.value = 1f;
        }


        public void SetImage(Texture2D texture) {
            _zoomSLider.value = 1f;

            if (texture != null) {
                _previewImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                _zoomSLider.value = 1f;
            } else {
                _previewImage.sprite = _defautlSprite;
                _zoomSLider.value = 1f;
            }
        }

        private void OnZoomInButtonClicked() {
            _zoomSLider.value += 0.1f;
        }

        private void OnZoomOutButtonClicked() {
            _zoomSLider.value -= 0.1f;
        }

        private void OnZoomSliderValueChanged(float value) {
            _previewImage.rectTransform.localScale = new Vector3(value, value, 1);
        }

        internal async void SaveTexture(string addrlink)
        {
            Debug.Log($"Path: {addrlink}");
            await UniTask.Yield(PlayerLoopTiming.Update);

            // Получаем информацию об обрезке
            CropResult cropResult = CropTexture();
            
            // Получаем исходную текстуру из спрайта
            Texture2D sourceTexture = _previewImage.sprite.texture;
            
            // Рассчитываем новые размеры текстуры
            int originalWidth = sourceTexture.width;
            int originalHeight = sourceTexture.height;
            
            // Вычисляем новые размеры с учетом обрезки/добавления полей
            int newWidth = originalWidth;
            int newHeight = originalHeight;
            
            // Рассчитываем размеры и позиции отдельно для каждой оси
            float horizontalScale = 1f;
            float verticalScale = 1f;
            
            if (cropResult.LeftCrop < 0 || cropResult.RightCrop < 0)
            {
                // Добавляем поля по горизонтали
                float totalPadding = Mathf.Abs(cropResult.LeftCrop) + Mathf.Abs(cropResult.RightCrop);
                horizontalScale = 1 + totalPadding;
                newWidth = Mathf.RoundToInt(originalWidth * horizontalScale);
            }
            else
            {
                // Обрезаем по горизонтали
                float totalCrop = cropResult.LeftCrop + cropResult.RightCrop;
                horizontalScale = 1 - totalCrop;
                newWidth = Mathf.RoundToInt(originalWidth * horizontalScale);
            }
            
            if (cropResult.TopCrop < 0 || cropResult.BottomCrop < 0)
            {
                // Добавляем поля по вертикали
                float totalPadding = Mathf.Abs(cropResult.TopCrop) + Mathf.Abs(cropResult.BottomCrop);
                verticalScale = 1 + totalPadding;
                newHeight = Mathf.RoundToInt(originalHeight * verticalScale);
            }
            else
            {
                // Обрезаем по вертикали
                float totalCrop = cropResult.TopCrop + cropResult.BottomCrop;
                verticalScale = 1 - totalCrop;
                newHeight = Mathf.RoundToInt(originalHeight * verticalScale);
            }
            
            // Создаем новую текстуру
            Texture2D newTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
            
            // Заполняем прозрачными пикселями
            Color[] clearPixels = new Color[newWidth * newHeight];
            for (int i = 0; i < clearPixels.Length; i++)
            {
                clearPixels[i] = new Color(0, 0, 0, 0);
            }
            newTexture.SetPixels(clearPixels);
            
            // Вычисляем позиции для копирования
            int destX = 0;
            int destY = 0;
            int srcX = 0;
            int srcY = 0;
            int copyWidth = originalWidth;
            int copyHeight = originalHeight;
            
            // Расчет позиций по горизонтали
            if (cropResult.LeftCrop < 0)
            {
                destX = Mathf.RoundToInt(Mathf.Abs(cropResult.LeftCrop) * originalWidth);
            }
            else
            {
                srcX = Mathf.RoundToInt(cropResult.LeftCrop * originalWidth);
                copyWidth = Mathf.RoundToInt(originalWidth * horizontalScale);
            }
            
            // Расчет позиций по вертикали
            if (cropResult.BottomCrop < 0)
            {
                destY = Mathf.RoundToInt(Mathf.Abs(cropResult.BottomCrop) * originalHeight);
            }
            else
            {
                srcY = Mathf.RoundToInt(cropResult.BottomCrop * originalHeight);
                copyHeight = Mathf.RoundToInt(originalHeight * verticalScale);
            }
            
            // Проверяем и корректируем размеры копирования
            copyWidth = Mathf.Min(copyWidth, originalWidth - srcX);
            copyHeight = Mathf.Min(copyHeight, originalHeight - srcY);
            
            // Копируем пиксели из исходной текстуры
            Color[] pixels = sourceTexture.GetPixels(srcX, srcY, copyWidth, copyHeight);
            newTexture.SetPixels(destX, destY, copyWidth, copyHeight, pixels);
            newTexture.Apply();
            
            // Сохраняем результат в файл
            byte[] bytes = newTexture.EncodeToPNG();
            await File.WriteAllBytesAsync(addrlink, bytes);
            
            // Очищаем ресурсы
            Destroy(newTexture);
        }

        public struct CropResult
        {
            public float LeftCrop;   // Отрицательное значение - добавить поля, положительное - обрезать
            public float RightCrop;
            public float TopCrop;
            public float BottomCrop;
            public bool NeedsCropping => LeftCrop != 0 || RightCrop != 0 || TopCrop != 0 || BottomCrop != 0;
        }

        public CropResult CropTexture()
        {
            CropResult result = new CropResult();
            
            // Получаем размеры и позиции в экранных координатах
            RectTransform previewRect = _previewImage.rectTransform;
            RectTransform maskRect = _maskImage.rectTransform;
            
            // Получаем границы прямоугольников в мировых координатах
            Vector3[] previewCorners = new Vector3[4];
            Vector3[] maskCorners = new Vector3[4];
            previewRect.GetWorldCorners(previewCorners);
            maskRect.GetWorldCorners(maskCorners);
            
            // Находим размеры в мировых координатах
            float previewWidth = Vector3.Distance(previewCorners[0], previewCorners[3]);
            float previewHeight = Vector3.Distance(previewCorners[0], previewCorners[1]);
            float maskWidth = Vector3.Distance(maskCorners[0], maskCorners[3]);
            float maskHeight = Vector3.Distance(maskCorners[0], maskCorners[1]);
            
            // Находим центры прямоугольников
            Vector3 previewCenter = previewRect.position;
            Vector3 maskCenter = maskRect.position;
            
            // Рассчитываем разницу между центрами
            float horizontalDiff = maskCenter.x - previewCenter.x;
            float verticalDiff = maskCenter.y - previewCenter.y;
            
            // Рассчитываем необходимую обрезку или добавление полей
            // По горизонтали
            if (previewWidth > maskWidth)
            {
                // Нужно обрезать
                float excess = (previewWidth - maskWidth) / 2;
                float relativeExcess = excess / previewWidth;
                result.LeftCrop = relativeExcess + (horizontalDiff / previewWidth);
                result.RightCrop = relativeExcess - (horizontalDiff / previewWidth);
            }
            else
            {
                // Нужно добавить поля
                float padding = (maskWidth - previewWidth) / 2;
                float relativePadding = padding / previewWidth;
                result.LeftCrop = -relativePadding - (horizontalDiff / previewWidth);
                result.RightCrop = -relativePadding + (horizontalDiff / previewWidth);
            }
            
            // По вертикали
            if (previewHeight > maskHeight)
            {
                // Нужно обрезать
                float excess = (previewHeight - maskHeight) / 2;
                float relativeExcess = excess / previewHeight;
                result.TopCrop = relativeExcess + (verticalDiff / previewHeight);
                result.BottomCrop = relativeExcess - (verticalDiff / previewHeight);
            }
            else
            {
                // Нужно добавить поля
                float padding = (maskHeight - previewHeight) / 2;
                float relativePadding = padding / previewHeight;
                result.TopCrop = -relativePadding - (verticalDiff / previewHeight);
                result.BottomCrop = -relativePadding + (verticalDiff / previewHeight);
            }
            
            return result;
        }

        private bool IsPixelCoveredByMask(int x, int y, RectTransform maskRect) {
            // Проверяем, покрыт ли пиксель маской или ее дочерними элементами
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(maskRect, new Vector2(x, y), null, out localPoint);
            return maskRect.rect.Contains(localPoint);
        }

        private void OnDrawGizmos() {
            if (_shotRect != null) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_shotRect.center, _shotRect.size);
            }
            if (_overlapRect != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_overlapRect.center, _overlapRect.size);
            }
        }
    }

}
