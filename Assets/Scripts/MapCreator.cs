using System;
using Unity.Netcode;
using UnityEngine;

namespace User.Jeffrey.Scripts.MapGenerator
{
    public class MapCreator : NetworkBehaviour
    {
        private MapCell[][] map;
        private const float MAP_CELL_SIZE = 10f;
        private const float ROAD_OFFSET = 0.5f;
        private const float ROAD_HIGHT_OFFSET = -0.5f;
        
        [SerializeField] private GameObject _mapPrefab;
        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject _roadPrefab;
        [SerializeField] private GameObject _KeyPrefab;
        [SerializeField] private GameObject _Exitrefab;
        
        private readonly NetworkVariable<int> _mapSeed = new NetworkVariable<int>();
        
        public override void OnNetworkSpawn()
        {
            _mapSeed.OnValueChanged += OnSeedChanged;

            if (IsServer)
            {
                int randomSeed = UnityEngine.Random.Range(0, 999999);
            
                _mapSeed.Value = randomSeed; 
            }
            else
            {
                if (_mapSeed.Value != 0)
                {
                    GenerateMapFromSeed(_mapSeed.Value);
                }
            }
        }
        private void OnSeedChanged(int previousValue, int newValue)
        {
            GenerateMapFromSeed(newValue);
        }

        private void GenerateMapFromSeed(int seed)
        {
            ClearCurrentMap();

            Debug.Log($"[MapCreator] 開始根據種子 {seed} 生成同步迷宮...");
            
            MapBuilder builder = new MapBuilder();

            map = builder.SetMapSize(10)
                .SetMapCellSize(10)
                .SetSeed(seed)
                .Initialize()
                .GenerateMap()
                .GenerateKey()
                .RemoveRandomWall()
                .Build();

            CreateMapView();
        }
        
        private void ClearCurrentMap()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        private void CreateMapView()
        {
            foreach (var mapX in map)
            {
                foreach (var MapY in mapX)
                {
                    SpawnMapView(MapY, true);
                }
            }
        }

        private void SpawnMapView(MapCell mapCell,bool spawnRoad)
        {
            mapCell.GetMapIndex(out int posX, out int posY);
                    
            Vector3 worldPos = new Vector3(posX * MAP_CELL_SIZE + MAP_CELL_SIZE/2.0f,
                0,
                posY * MAP_CELL_SIZE + MAP_CELL_SIZE/2.0f);
            var g = Instantiate(_mapPrefab);
            g.transform.localScale = new Vector3(MAP_CELL_SIZE,MAP_CELL_SIZE,MAP_CELL_SIZE);
            g.transform.position = worldPos;
            g.transform.rotation = Quaternion.Euler(90, 0, 0);
            g.name = $"Map {posX}/{posY}";
            if (mapCell.HasKey)
            {
                SpawnKey(g.transform);
            }

            if (mapCell.IsExit)
            {
                SpawnExit(g.transform);
            }
            if (spawnRoad)
            {
                SpawnRoadWithMapCell(mapCell,g.transform);
            }
        }

        private void SpawnExit(Transform gTransform)
        {
            GameObject r = Instantiate(_Exitrefab, gTransform);
            r.transform.rotation = Quaternion.Euler(0, 0, 0);
            r.transform.localScale = Vector3.one * 0.2f;
            r.transform.localPosition = new Vector3(0, 0, -0.5f);
            r.name = "Exit";
        }

        private void SpawnKey(Transform gTransform)
        {
            GameObject r = Instantiate(_KeyPrefab, gTransform);
            r.transform.localScale = Vector3.one * 0.1f;
            r.transform.localPosition = new Vector3(0, 0, -0.6f);
            r.name = "Key";
        }

        private void SpawnRoadWithMapCell(MapCell mapCell,Transform parent)
        {
            SpawnRoadIfHas(mapCell.HasUpRoad, "Up", new Vector3(0, ROAD_OFFSET,ROAD_HIGHT_OFFSET), 90, parent);
            SpawnRoadIfHas(mapCell.HasDownRoad, "Down", new Vector3(0, -ROAD_OFFSET, ROAD_HIGHT_OFFSET), 90, parent);
            SpawnRoadIfHas(mapCell.HasLeftRoad, "Left", new Vector3(-ROAD_OFFSET, 0, ROAD_HIGHT_OFFSET), 0, parent);
            SpawnRoadIfHas(mapCell.HasRightRoad, "Right", new Vector3(ROAD_OFFSET, 0, ROAD_HIGHT_OFFSET), 0, parent);
        }
        
        private void SpawnRoadIfHas(bool hasRoad, string roadName, Vector3 localPos, float rotationX, Transform parent)
        {
            GameObject r = Instantiate(hasRoad ? _roadPrefab : _wallPrefab, parent);
            r.transform.localPosition = localPos;
            // r.name = roadName;
            r.transform.localScale = new Vector3(1,1,0.05f);
            r.transform.localRotation = Quaternion.Euler(rotationX, 90, 0);
        }
    }
}