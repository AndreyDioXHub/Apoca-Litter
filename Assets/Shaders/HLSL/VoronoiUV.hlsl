
#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//#include "Random.cginc"
#endif

void VoronoiUV_half(
    in float2 UV,
    in float Speed,
    in float CellSize,
    in float Diver,
    in float CellOffcet,
    in float Csq,
    in float Step,
    in float Intencity,
    out float Out)
{
    float result = 0;

    half uOffset = 0;
    half vOffset = 0;

    float cover;

    float2 value = UV / CellSize;

    float2 baseCell = floor(value);

    float minDistToCell = 10;
    float2 toClosestCell;
    float2 closestCell;

    float2 rad = float2(0, 0);
    float kA = 2 * PI;
    float fA = 0;

    [unroll]
    for (int x1 = -1; x1 <= 1; x1++) {
        [unroll]
        for (int y1 = -1; y1 <= 1; y1++) {
            float2 cell = baseCell + float2(x1, y1);
            fA = kA * (frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453123) - Speed);
            //fA = kA * (1 - Speed);
            rad.x = 0.5 * cos(fA) + 0.5; //((Csq * cos(fA)) / (1 + sin(fA) * sin(fA)) - CellOffcet) / Diver; //0.5 * cos(fA) + 0.5; //2 * c - 2 * sq
            rad.y = 0.5 * sin(fA) + 0.5; //((Csq * sin(fA) * cos(fA)) / (1 + sin(fA) * sin(fA)) - CellOffcet) / Diver; //0.5 * sin(fA) + 0.5;

            float2 cellPosition = cell + rad;// +frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453123);
            float2 toCell = cellPosition - value;
            float distToCell = length(toCell);
            //result = distToCell;
            if (distToCell < minDistToCell) {
                minDistToCell = distToCell;
                closestCell = cell;
                toClosestCell = toCell;
            }
        }
    }

    //second pass to find the distance to the closest edge
    float minEdgeDistance = 10;
    [unroll]
    for (int x2 = -1; x2 <= 1; x2++) {
        [unroll]
        for (int y2 = -1; y2 <= 1; y2++) {
            float2 cell = baseCell + float2(x2, y2);
            fA = kA * (frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453123) - Speed);
            //rad.x = ((Csq * cos(fA)) / (1 + sin(fA) * sin(fA)) - CellOffcet) / Diver; //0.5 * cos(fA) + 0.5; //2 * c - 2 * sq
            //rad.y = ((Csq * sin(fA) * cos(fA)) / (1 + sin(fA) * sin(fA)) - CellOffcet) / Diver; //0.5 * sin(fA) + 0.5;
            rad.x = 0.5 * cos(fA) + 0.5;
            rad.y = 0.5 * sin(fA) + 0.5;
            float2 cellPosition = cell + rad;// +frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453123);
            float2 toCell = cellPosition - value;

            float2 diffToClosestCell = abs(closestCell - cell);
            bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y < 0.1;
            if (!isClosestCell) {
                float2 toCenter = (toClosestCell + toCell) * 0.5;
                float2 cellDifference = normalize(toCell - toClosestCell);
                float edgeDistance = dot(toCenter, cellDifference);
                minEdgeDistance = min(minEdgeDistance, edgeDistance);
            }
        }
    }

    float noise = step(minEdgeDistance, Step);

    result = Intencity * noise;

    Out = result;
}