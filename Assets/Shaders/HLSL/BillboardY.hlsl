void BillboardY_float(
    float3 VertexPosition,
    float3 ObjectWorldPosition,
    float3 CameraWorldPosition,
    out float3 Out
)
{
    // Вычисляем направление к камере в мировом пространстве
    float3 toCamera = CameraWorldPosition - ObjectWorldPosition;
    
    // Обнуляем Y-компоненту для вращения только по горизонтали
    toCamera.y = 0;
    toCamera = normalize(toCamera);
    
    // Вычисляем угол поворота
    float angle = atan2(toCamera.x, toCamera.z);
    
    // Создаем матрицу вращения вокруг оси Y
    float sinAngle, cosAngle;
    sincos(angle, sinAngle, cosAngle);
    
    float4x4 rotationMatrix = float4x4(
        cosAngle, 0, sinAngle, 0,
        0, 1, 0, 0,
        -sinAngle, 0, cosAngle, 0,
        0, 0, 0, 1
    );
    
    // Применяем вращение к вершине
    Out = mul(rotationMatrix, float4(VertexPosition, 1.0)).xyz;
}