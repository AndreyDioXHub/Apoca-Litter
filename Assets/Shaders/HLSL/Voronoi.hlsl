
#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    //#include "Random.cginc"
#endif

void Voronoi_half(
    in float4 Position,
    in float Speed,
    in float CellSize,
    out float Out) 
{
    float2 value = Position.xy / CellSize;
    /*value.x += Speed.x;
    value.y += Speed.y;*/
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
            rad.x = 0.5 * cos(fA) + 0.5;
            rad.y = 0.5 * sin(fA) + 0.5;

            float2 cellPosition = cell + rad;// +frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453123);
            float2 toCell = cellPosition - value;
            float distToCell = length(toCell);
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

    float noise = step(minEdgeDistance, 0.05);

    Out = noise;
}
/* {
    float2 value = Position.xy / CellSize;

    float2 cell = floor(value);
    //float xrand = random(0,cell.x);
    float2 rad = float2(0,0);
    float kA = 2 * PI;
    float fA = kA * (1 - Speed);
    rad.x = 0.5 * cos(fA) + 0.5;
    rad.y = 0.5 * sin(fA) + 0.5;

    float2 cellPosition = cell + rad;// +frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453123); 
    float2 toCell = cellPosition - value;
    float distToCell = length(toCell);

    float noise = distToCell;

    Out = noise;
}
/*

    /*
{
    float2 value = Position.xy / CellSize;

    float2 cell = floor(value);
    //float xrand = random(0,cell.x);
    float2 cellPosition = cell + frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453123); //Random / 10;// rand2dTo2d(cell);
    float2 toCell = cellPosition - value;
    float distToCell = length(toCell);

    float noise = distToCell;

    Out = noise;
}*/
/*
float voronoiNoise(float2 value)
{
    float2 cell = floor(value);
    float2 cellPosition = cell + cell / 2;// rand2dTo2d(cell);
    float2 toCell = cellPosition - value;
    float distToCell = length(toCell);
    return distToCell;
}*/