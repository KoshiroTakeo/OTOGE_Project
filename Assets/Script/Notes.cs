using System.Collections;
using UnityEngine;
using UniRx;          //UniRX使用
using UniRx.Triggers; //UniRXの??使用

public class Notes : MonoBehaviour
{

    GameController GameController; //特定スクリプト参照用変数
    AudioSource NotesSE;           //音楽データ
    public AudioClip HitSE;        //ノーツを叩いたときの音
    bool UsedHit = false;          //使い終わってるか銅か
    bool GoNotes = false;


    char Type;                     //ノーツのタイプ
    bool isInLine = false;         //ノーツ判定
    public int lineNum;            //レーン番号
    [SerializeField]float Timing;                    //タイミング

    KeyCode LineKey;               //ノーツ入力キー

    private float BPMSpeed;                  //ノーツの落ちる初期速度
   

    
    



    void Start()
    {
        //シーン上のゲームコントローラーを取得し、各要素を入れる***********************************
        GameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        BPMSpeed = GameController.GetComponent<GameController>().Beat;       //10かけたらいい感じ       
        //*****************************************************************************************


        LineKey = GameUtil.GetKeyCodeByLineNum(lineNum);

        //プレハブ内のAudioSourceを取得する********************************************************
        NotesSE = this.gameObject.GetComponent<AudioSource>();
        //*****************************************************************************************
        //HighSpeedChange = true;
    }



    void Update()
    {
        
        if (GameController.ReciveFlame() >= Timing && GoNotes == false)
        {
            
            
            GoNotes = true;
        }
        
        if (GoNotes == true)
        {
            this.transform.position += new Vector3(0, -1, 0);
        }

        

        //ノーツが消える処理
        NotesDestroy();
        
    }



    void OnTriggerEnter(Collider other)        //コライダー内にノーツが入った時
    {

        isInLine = true;
    }

    void OnTriggerExit(Collider other)         //コライダー内からノーツが出た時
    {
        isInLine = false;
    }

    void CheckInput(KeyCode key)                 //押したとき
    {

        if (Input.GetKeyDown(key))
        {
            //GameController.GoodTimingFunc(lineNum);
            Destroy(this.gameObject);
        }
    }

    void NotesDestroy()
    {
       
        //取り損ねたノーツの消える位置
        if (this.transform.position.y < -210.0f)
        {
            //Debug.Log("false");

            Destroy(this.gameObject);
        }
        //判定位置でノーツを押したとき
        if (isInLine)
        {
            Debug.Log("Hit");
            CheckInput(LineKey);
        }
        //オート
        if (this.transform.position.y <= -190.0f && !UsedHit)
        {
            //NotesSE.PlayOneShot(HitSE);
            
            UsedHit = true;
            
        }
    }



    public void setParameter(char type, float timing) //GManagerから「type」、「timing」値を受け取る
    {
        Type = type;
        Timing = timing;

    }



    //public float getTiming()
    //{
    //    //return Timing;
    //}
}

//20200430 timing要調整（float部の仕様変更のため）
