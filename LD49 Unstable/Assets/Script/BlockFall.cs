using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFall : MonoBehaviour
{
    private bool grounded = false;

    private float currFallFor;
    private float speed;

    private float rotate;
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    private bool isRotating = false;
    private bool canRotate = true;

    private float move;

    public int level;
    public Mesh[] meshes;

    public float moveInterval;

    private bool canMove = true;

    public MeshFilter projectionMeshFilter;
    public MeshRenderer projectionRend;
    public float projectionAlphaMult;

    private MeshFilter meshFilter;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        #region Transparent Material

        //projectionRend.material.SetFloat("_Mode", 2);
        //projectionRend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //projectionRend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //projectionRend.material.SetInt("_ZWrite", 0);
        //projectionRend.material.DisableKeyword("_ALPHATEST_ON");
        //projectionRend.material.EnableKeyword("_ALPHABLEND_ON");
        //projectionRend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //projectionRend.material.renderQueue = 3000;

        //projectionRend.material.color = new Color(projectionRend.material.color.r, projectionRend.material.color.g, projectionRend.material.color.b, projectionRend.material.color.a * projectionAlphaMult);

        #endregion

        //transform.position += (transform.position * (2 - level)) / 2;
        transform.parent.eulerAngles = new Vector3(0, Random.Range(0, 5) * 60, 0);
        //meshFilter.mesh = meshes[level];

        projectionMeshFilter.mesh = meshFilter.mesh;
        projectionRend.gameObject.SetActive(false);

    }

    void Update()
    {
        speed = Input.GetAxisRaw("Speed");
        move = Input.GetAxisRaw("Vertical");

        if (Input.touchCount != 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPos = touch.position / new Vector2(Screen.width, Screen.height);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPos = touch.position / new Vector2(Screen.width, Screen.height);

                if (!grounded && canRotate)
                {
                    if (Vector2.Distance(startTouchPos, endTouchPos) > BlockManager.blockManager.touchSwipeLimit)
                    {
                        rotate = 0;
                        Debug.Log("swiped");
                    }
                    else
                    {
                        if (endTouchPos.x < BlockManager.blockManager.sideThreshold) //sufficiently left
                        {
                            rotate = -1;
                        }
                        else if (endTouchPos.x > (1 - BlockManager.blockManager.sideThreshold)) //sufficiently right
                        {
                            rotate = 1;
                        }
                        else
                        {
                            Debug.Log("not on right or left");
                            return;
                        }

                        Debug.Log("start rotate");
                        StartCoroutine(Rotate());
                        StartCoroutine(StopRotate());
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        Fall();
    }

    void Fall()
    {
        if (!grounded)
        {
            if (speed != 0)
            {
                if (speed > 0)
                {
                    currFallFor = BlockManager.blockManager.fallFor * BlockManager.blockManager.slowMult;
                }
                else
                {
                    currFallFor = BlockManager.blockManager.fallFor * BlockManager.blockManager.fastMult;
                }
            }
            else
            {
                currFallFor = BlockManager.blockManager.fallFor;
            }

            //rb.velocity = Vector2.down * (BlockManager.blockManager.fallDist / currFallFor);
            //transform.position += Vector3.down * (BlockManager.blockManager.fallDist / currFallFor) * Time.deltaTime;
            rb.MovePosition(rb.position + Vector3.down * (BlockManager.blockManager.fallDist / currFallFor) * Time.deltaTime);
        }
    }

    //IEnumerator Move()
    //{
    //    canMove = false;

    //    int levelDest = level += Mathf.RoundToInt(move);
    //    Vector3 moveDir = new Vector3(-transform.position.x, 0, -transform.position.z);

    //    if (levelDest >= 0 && levelDest <= 2)
    //    {
    //        transform.position += (move * moveDir) / level;
    //        meshFilter.mesh = meshes[levelDest];
    //        level = levelDest;
    //    }

    //    yield return new WaitForSeconds(moveInterval);

    //    canMove = true;
    //}

    IEnumerator Rotate()
    {
        canRotate = false;
        isRotating = true;

        float rotateDir = rotate;

        while (isRotating && !grounded)
        {
            transform.parent.Rotate(Vector3.up, rotateDir * (60 / BlockManager.blockManager.rotateFor) * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator StopRotate()
    {
        yield return new WaitForSeconds(BlockManager.blockManager.rotateFor);

        if (isRotating)
        {
            isRotating = false;
            transform.parent.eulerAngles = new Vector3(0, (Mathf.RoundToInt(transform.parent.eulerAngles.y / 60)) * 60, 0);

            yield return new WaitForSeconds(BlockManager.blockManager.inBetweenRotates);
            canRotate = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isRotating = false;
        canRotate = false;

        projectionRend.gameObject.SetActive(false);

        rb.useGravity = true;

        grounded = true;

        if (Spawner.spwn.spawnedBlocks[0] == transform.parent.gameObject)
        {
            Debug.Log("hello");
            Spawner.spwn.Spawn();
            Spawner.spwn.spawnedBlocks.RemoveAt(0);
        }

        if (Spawner.spwn.highestBlock <= transform.position.y)
        {
            Spawner.spwn.highestBlock = transform.position.y;
        }

        enabled = false;
    }

}
