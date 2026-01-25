using UnityEngine;

public class TiledBackground : MonoBehaviour
{
    [System.Serializable]
    public class TileOption
    {
        public Sprite tileSprite;
        [Range(0, 100)] public float spawnChance = 10f;
    }

    [SerializeField] private TileOption[] tileOptions;
    [SerializeField] private int padding = 2;
    [SerializeField] private string sortingLayerName = "Background";
    [SerializeField] private int sortingOrder = 0;

    private Camera mainCamera;
    private int gridWidth, gridHeight;
    private Vector2Int gridOffset;
    private SpriteRenderer[,] tiles;

    void Start()
    {
        mainCamera = Camera.main;

        float camHeight = mainCamera.orthographicSize * 2;
        float camWidth = camHeight * mainCamera.aspect;

        gridWidth = Mathf.CeilToInt(camWidth) + padding * 2;
        gridHeight = Mathf.CeilToInt(camHeight) + padding * 2;

        tiles = new SpriteRenderer[gridWidth, gridHeight];

        Vector2 cameraPos = mainCamera.transform.position;
        gridOffset = new Vector2Int(
            Mathf.FloorToInt(cameraPos.x),
            Mathf.FloorToInt(cameraPos.y)
        );

        SpawnInitialTiles();
    }

    void SpawnInitialTiles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                SpawnTile(x, y);
            }
        }
    }

    void SpawnTile(int gridX, int gridY)
    {
        int worldX = gridX + gridOffset.x - gridWidth / 2;
        int worldY = gridY + gridOffset.y - gridHeight / 2;

        Sprite sprite = PickRandomSprite();

        if (sprite != null)
        {
            GameObject tile = new GameObject($"Tile_{worldX}_{worldY}");
            tile.transform.position = new Vector3(worldX, worldY, 10);
            tile.transform.parent = transform;

            SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;

            tiles[gridX, gridY] = sr;
        }
    }

    Sprite PickRandomSprite()
    {
        if (tileOptions == null || tileOptions.Length == 0)
            return null;

        float totalWeight = 0f;
        foreach (var option in tileOptions)
        {
            totalWeight += option.spawnChance;
        }

        float randomValue = Random.Range(0f, totalWeight);

        float currentWeight = 0f;
        foreach (var option in tileOptions)
        {
            currentWeight += option.spawnChance;
            if (randomValue <= currentWeight)
            {
                return option.tileSprite;
            }
        }

        return tileOptions[0].tileSprite;
    }

    public void RecenterBackground()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (tiles[x, y] != null)
                    Destroy(tiles[x, y].gameObject);
            }
        }

        Vector2 cameraPos = mainCamera.transform.position;
        gridOffset = new Vector2Int(
            Mathf.FloorToInt(cameraPos.x),
            Mathf.FloorToInt(cameraPos.y)
        );

        SpawnInitialTiles();
    }

    void Update()
    {
        Vector2 currentCameraPos = mainCamera.transform.position;
        Vector2Int currentGridOffset = new Vector2Int(
            Mathf.FloorToInt(currentCameraPos.x),
            Mathf.FloorToInt(currentCameraPos.y)
        );

        Vector2Int delta = currentGridOffset - gridOffset;

        if (Mathf.Abs(delta.x) > gridWidth / 2 || Mathf.Abs(delta.y) > gridHeight / 2)
        {
            RecenterBackground();
        }
        else if (currentGridOffset != gridOffset)
        {
            ShiftGrid(delta);
            gridOffset = currentGridOffset;
        }
    }

    void ShiftGrid(Vector2Int delta)
    {
        if (delta.x > 0)
        {
            for (int i = 0; i < delta.x; i++)
                ShiftRight();
        }
        else if (delta.x < 0)
        {
            for (int i = 0; i < -delta.x; i++)
                ShiftLeft();
        }

        if (delta.y > 0)
        {
            for (int i = 0; i < delta.y; i++)
                ShiftUp();
        }
        else if (delta.y < 0)
        {
            for (int i = 0; i < -delta.y; i++)
                ShiftDown();
        }
    }

    void ShiftRight()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (tiles[0, y] != null)
                Destroy(tiles[0, y].gameObject);
        }

        for (int x = 0; x < gridWidth - 1; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                tiles[x, y] = tiles[x + 1, y];
            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            SpawnTile(gridWidth - 1, y);
        }
    }

    void ShiftLeft()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (tiles[gridWidth - 1, y] != null)
                Destroy(tiles[gridWidth - 1, y].gameObject);
        }

        for (int x = gridWidth - 1; x > 0; x--)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                tiles[x, y] = tiles[x - 1, y];
            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            SpawnTile(0, y);
        }
    }

    void ShiftUp()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (tiles[x, 0] != null)
                Destroy(tiles[x, 0].gameObject);
        }

        for (int y = 0; y < gridHeight - 1; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                tiles[x, y] = tiles[x, y + 1];
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            SpawnTile(x, gridHeight - 1);
        }
    }

    void ShiftDown()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (tiles[x, gridHeight - 1] != null)
                Destroy(tiles[x, gridHeight - 1].gameObject);
        }

        for (int y = gridHeight - 1; y > 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                tiles[x, y] = tiles[x, y - 1];
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            SpawnTile(x, 0);
        }
    }
}