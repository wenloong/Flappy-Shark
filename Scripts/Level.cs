using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.5f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION = -100f;
    private const float PIPE_SPAWN_X_POSITION = 100f;
    private const float SHARK_X_POSITION = 0f;

    private int pipePassedCount;
    private int pipeSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;

    private static Level instance;
    private State state;

    public static Level GetInstance()
    {
        return instance;
    }

    private List<Pipe> pipeList;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        SharkDead,
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 2f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }
    private void Start()
    {
        Shark.GetInstance().OnDied += Shark_OnDied;
        Shark.GetInstance().OnStartPlaying += Shark_OnStartPlaying;
    }

    private void Update()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
        
    }

    private void Shark_OnDied(object sender, System.EventArgs e)
    {
        state = State.SharkDead;
    }

    private void Shark_OnStartPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void HandlePipeSpawning()
    {
        
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

            float height = UnityEngine.Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION);
            pipeSpawned++;
            SetDifficulty(GetDifficulty());
        }
    }

    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRight = pipe.GetXPosition() > SHARK_X_POSITION;
            pipe.Move();
            
            if (isToTheRight && pipe.GetXPosition() <= SHARK_X_POSITION && pipe.IsBottom())
            {
                pipePassedCount++;
            }

            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)
            {
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 40f;
                break;
            case Difficulty.Medium:
                gapSize = 30f;
                break;
            case Difficulty.Hard:
                gapSize = 20f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipeSpawned >= 20) return Difficulty.Hard;
        if (pipeSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
    }

    private void CreatePipe(float height, float xPosition, bool createBottom) 
    {
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);

        float pipeHeadYPosition;

        if (createBottom)
        {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        } else
        {
            pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);
        
        float pipeBodyYPosition;

        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        } else
        {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);
        pipeList.Add(pipe);
    }

    public int GetPipeSpawned()
    {
        return pipeSpawned;
    }

    public int GetPipePassedCount()
    {
        return pipePassedCount;
    }



    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public bool IsBottom()
        {
            return isBottom;
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
