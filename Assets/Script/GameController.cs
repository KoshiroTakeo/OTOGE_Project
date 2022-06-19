using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.UI;
using UniRx;          //UniRX使用
using UniRx.Triggers; //UniRXの??使用

public class GameController : MonoBehaviour
{
    

    

    public GameObject StartButton;
  
    
    private float NotesOffset;          //ノーツが最初に判定ラインに到達させる時間
    private float StartMusisTime;       //音楽をかける時間

    //ノーツを生成する**********************************************
    private List<GameObject> JsonNotes; //1曲分のノーツデータを格納
    private GameObject Note;
    private JsonNode json;

    private string Title;               //曲のタイトルを格納
    private string BarData;             //1小節内のノーツデータ
    private char   type;                //ノーツの種類   
    private float BPM;                  //BPMを判別
    public float Beat;                 //1拍の時間   
    private float NotesTiming;        //ノーツの間隔

    private float GameTime;
    public float CountTime;            //楽曲プレイ時間
    private int CountNoteLine;            //1ラインのノーツ数カウント
    private int Bar;                  //小節カウント
    
    
   
   

    private int CharCount; //文字カウント

    private int CountNoteLines;            //1小節内のノーツデータ数カウント

    private int Tempo; //拍子



    private AudioSource AudioSource;    //音楽データ

    private bool IsPlaying = false;
    private bool NotesComp = false;
    private bool OnMusic = false;

    [SerializeField] string FilePath;   //読み込むJsonファイル
    [SerializeField] string ClipPath;   //楽曲データ
    //**************************************************************



    //ノーツを生成する場所**************************************
    [SerializeField] GameObject NotesPrefab;     //ノーツ
    [SerializeField] Transform NotesPos_A;       //生成場所
    [SerializeField] Transform NotesPos_B;       //生成場所
    [SerializeField] Transform NotesPos_C;       //生成場所
    [SerializeField] Transform NotesPos_D;       //生成場所
    //**********************************************************






    //初期化================================================================================
    void Start()
    {
        AudioSource =this.gameObject.GetComponent<AudioSource>();
        GameTime = 0.0f;
        CountTime = 0.0f;       
        LoadChart();                  //Json読込
        IsPlaying = true;             //プレイ中切り替え

        //生成場所を取得*********************************************************
       
        NotesPos_A = GameObject.FindWithTag("SpawnPos_A").gameObject.transform;
        NotesPos_B = GameObject.FindWithTag("SpawnPos_B").gameObject.transform;
        NotesPos_C = GameObject.FindWithTag("SpawnPos_C").gameObject.transform;
        NotesPos_D = GameObject.FindWithTag("SpawnPos_D").gameObject.transform;
        //***********************************************************************

        //ノーツ生成に関する初期化***********************************************
        Bar = 0;
        Tempo = 4; //4拍子を基準とする
        NotesTiming = 0.0f;
        //***********************************************************************
    }
    //======================================================================================




    //更新処理******************************************************************************
    void Update()
    {
        //ゲーム開始
        if (IsPlaying)
        {
            //ゲーム中の時間を数え始める（本当はフレームで管理したい）
            GameTime += Time.deltaTime;

            //時間になったら音楽を再生する
            StartMusic(CountTime);

            //BPMからノーツの間隔を生成
            SetBeat();

            //ノーツの出力が完了してなければ
            if(GameTime >= NotesOffset)
            {
                CountTime += Time.deltaTime;
                SetNotes();            
            }          
            //フレーム時間になったらノーツを出現させる

            
        }
    }
    //**************************************************************************************



    //BPMによる基本スピードを設定***********************************************************
    void SetBeat()
    {
        Beat = (60 / BPM); //ノーツが流れる間隔
  
    }
   


    //Json譜面データの読み込み部==========================================================================================================
    void LoadChart()
    {
        JsonNotes = new List<GameObject>();  //

        //読込
        string jsonText = Resources.Load<TextAsset>("Charts\\" + FilePath).ToString();  //jsonTextに、Chart内のFilePathのデータを入れる       
        AudioSource.clip = (AudioClip)Resources.Load("Charts\\" + ClipPath);             //Music.clipに、Chart内のClipPathのデータを入れる
        
        //解読
        json = JsonNode.Parse(jsonText);    //jsonText(FilePath)に入れたJsonファイルを展開する
        

        //割り当て========================================================================================================================
        // "コマンドテキスト": "指示", で読み込む
        Title          = json["TITLE"].Get<string>();                    //タイトル (例 "title": に続く「文字」を「,」まで入れる)       
        BPM            = int.Parse(json["BPM"].Get<string>());           //BPM                                          
        NotesOffset    = float.Parse(json["OFFSET"].Get<string>());      //ノーツを流し始める時間
        StartMusisTime = float.Parse(json["STARTMUSIC"].Get<string>());  //曲を流し始める時間
        //================================================================================================================================
    }
    //====================================================================================================================================


    
    //ノーツを生成する===========================================================================================================================
    void SetNotes()
    {
        if (!NotesComp)
        {
        　　int i = 0;
        　　int j = 0;
        　　int cnt = 0;
        　　float space;

            int Count = 0;

        //SetNotesが終わったら実行しない
        
            //ノーツの識別========================================================================================
            foreach (var note in json["SCORE"]) //"SCORE"(譜面)に続く配列を確認する                           
            {

                //小節毎に譜面データを解析する
                Debug.Log(Count++);
                Bar++;                                              //小節数をカウント                   
                BarData   = note["NOTE"].Get<string>();             //「NOTE」に続く文字の要素を取得                      
                CharCount = BarData.Length;                         //BarData文字数取得
                
                //取得した要素から出現させる音符ラインの数を数える
                for (i = 0, CountNoteLine = 0; i < CharCount; i++)
                {
                    if ((type = BarData[i]) == ',') CountNoteLine++; //「,」で１小節に何列音符ができるか確認する
                }

               
                space = ((Beat * Tempo) / CountNoteLine); // その小節での音符の列同士の間隔
                
                

                //ノーツ出力部========================================================================================================
                for (i = 0, j = 0; i < CharCount; i++)
                {
                    type = BarData[i];                 

                    if (!(type == ',')) NotesTiming += space ;

                    //ノーツ生成*******************************************************************************************************
                    switch (type)
                    {
                        case '1':
                            Note = Instantiate(NotesPrefab, new Vector3(NotesPos_A.position.x,  NotesPos_A.position.y, NotesPos_A.position.z), Quaternion.identity);
                            Note.GetComponent<Notes>().setParameter(type, NotesTiming);
                            Debug.Log("正常[レーン1]");
                            break;
                        case '2':
                            Note = Instantiate(NotesPrefab, new Vector3(NotesPos_B.position.x, NotesPos_B.position.y, NotesPos_B.position.z), Quaternion.identity);
                            Note.GetComponent<Notes>().setParameter(type, NotesTiming);
                            Debug.Log("正常[レーン2]");
                            break;
                        case '3':
                            Note = Instantiate(NotesPrefab, new Vector3(NotesPos_C.position.x, NotesPos_C.position.y, NotesPos_C.position.z), Quaternion.identity);
                            Note.GetComponent<Notes>().setParameter(type, NotesTiming);
                            Debug.Log("正常[レーン3]");
                            break;
                        case '4':
                            Note = Instantiate(NotesPrefab, new Vector3(NotesPos_D.position.x, NotesPos_D.position.y, NotesPos_D.position.z), Quaternion.identity);
                            Note.GetComponent<Notes>().setParameter(type, NotesTiming);
                            //Note = Instantiate(Notes, new Vector3(3.0f, NotesTiming, -1.1f), Quaternion.identity); //前のやつ
                            Debug.Log("正常[レーン4]");
                            break;
                        case '0':
                            Debug.Log("正常[休符]");
                            break;
                        case ',':
                            cnt++;
                            Debug.Log("正常[次のラインへ]");
                            break;
                        default:
                            Debug.LogError("範囲外のノーツが検出されました");
                            break;
                    }
                    //******************************************************************************************************************

                    
                    
                    ;
                    
                    
                }
                //======================================================================================================================
            }

            
            
            //完了する
            NotesComp = true;
        }
    }
    //============================================================================================================================================


    //フレーム時間からノーツを落とす********************************************************
    void GoNotes(int flame)
    {
       
    }
    


    //ゲーム開始=============================================================================
    public void StartGame()
    {
        StartButton.SetActive(false); //StartButtonを無効化

    }
    //=======================================================================================

    //曲を流す*******************************************************************************
    void StartMusic(float time)
    {
        if (!OnMusic)
        {
            if (StartMusisTime <= time)
            {
                AudioSource.Play();
                OnMusic = true;
            }
        }
    }
    //****************************************************************************************

    public float ReciveFlame()
    {
        return CountTime;
    }
}


//BPMが上がるほどノーツ間隔は狭くなる
//bpm60 = １分間に60 = １秒１拍
//time.deltatime 

//１小節4拍（曲によっては変わる）



//いる？？
////曲の尺=================================================================================
//float GetMusicTime()
//{
//    return Time.time - CountTime;
//}
////経過時間=================================================================================
//private void Update()
//{
//    Playtime = (Time.time - CountTime) * 1000;
//    Debug.Log(Playtime);
//}



////ノーツを生成する===========================================================================================================================
//void SetNotes()
//{
//    //SetNotesが終わったら実行しない
//    if (!NotesComp)
//    {
//        //ノーツの識別========================================================================================
//        foreach (var note in json["SCORE"]) //"SCORE"(譜面)に続く配列を確認する                           
//        {

//            Bar = int.Parse(note["BAR"].Get<string>());   //小節番号(:に続く値を読む)
//            BarData = note["NOTE"].Get<string>();             //音符番号                      
//            DataCount = BarData.Length;                         //1小節内の音符数


//            //音符の間隔をとるため１小節の「 ,」 を数える
//            for (NotesCount = 0, CountNoteLine = 0.0f; NotesCount < DataCount; NotesCount++)
//            {
//                type = BarData[NotesCount]; //配列中身を順に見る

//                if (type == ',')
//                {
//                    CountNoteLine++;
//                }
//            }


//            //ノーツ出力部========================================================================================================
//            for (NotesCount = 0, cnt = 0.0f; NotesCount < DataCount; NotesCount++)
//            {
//                type = BarData[NotesCount];
//                NotesTiming = (Bar + (cnt / CountNoteLine)) * 100;

//                //ノーツ生成*******************************************************************************************************
//                switch (type)
//                {
//                    case '1':
//                        Note = Instantiate(NotesPrefab, new Vector3(NotesPos_A.position.x, NotesTiming + NotesPos_A.position.y, NotesPos_A.position.z), Quaternion.identity);
//                        //Debug.Log("正常[レーン1]");
//                        break;
//                    case '2':
//                        Note = Instantiate(NotesPrefab, new Vector3(NotesPos_B.position.x, NotesTiming + NotesPos_B.position.y, NotesPos_B.position.z), Quaternion.identity);
//                        //Debug.Log("正常[レーン3]");
//                        break;
//                    case '3':
//                        Note = Instantiate(NotesPrefab, new Vector3(NotesPos_C.position.x, NotesTiming + NotesPos_C.position.y, NotesPos_C.position.z), Quaternion.identity);
//                        //Debug.Log("正常[レーン3]");
//                        break;
//                    case '4':
//                        Note = Instantiate(NotesPrefab, new Vector3(NotesPos_D.position.x, NotesTiming + NotesPos_D.position.y, NotesPos_D.position.z), Quaternion.identity);
//                        //Note = Instantiate(Notes, new Vector3(3.0f, NotesTiming, -1.1f), Quaternion.identity); //前のやつ
//                        //Debug.Log("正常[レーン4]");
//                        break;
//                    case '0':
//                        //Debug.Log("正常[休符]")
//                        break;
//                    case ',':
//                        cnt++;
//                        //Debug.Log("正常[次のラインへ]")
//                        break;
//                    default:
//                        Debug.LogError("範囲外のノーツが検出されました");
//                        break;
//                }
//                //******************************************************************************************************************

//                NotePosition = Note.transform.position.y;
//                Note.GetComponent<Notes>().setParameter(type, Bar, NotesTiming);
//            }
//            //======================================================================================================================
//        }
//        NotesComp = true;
//    }
//}
////============================================================================================================================================