void BillboardY_float(
    float3 VertexPosition,
    float3 ObjectWorldPosition,
    float3 CameraWorldPosition,
    out float3 Out
)
{
    // ��������� ����������� � ������ � ������� ������������
    float3 toCamera = CameraWorldPosition - ObjectWorldPosition;
    
    // �������� Y-���������� ��� �������� ������ �� �����������
    toCamera.y = 0;
    toCamera = normalize(toCamera);
    
    // ��������� ���� ��������
    float angle = atan2(toCamera.x, toCamera.z);
    
    // ������� ������� �������� ������ ��� Y
    float sinAngle, cosAngle;
    sincos(angle, sinAngle, cosAngle);
    
    float4x4 rotationMatrix = float4x4(
        cosAngle, 0, sinAngle, 0,
        0, 1, 0, 0,
        -sinAngle, 0, cosAngle, 0,
        0, 0, 0, 1
    );
    
    // ��������� �������� � �������
    Out = mul(rotationMatrix, float4(VertexPosition, 1.0)).xyz;
}