using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesLine : MonoBehaviour
{
    Transform LinePos;
    AudioSource NotesSE;           //音楽データ
    public AudioClip HitSE;        //ノーツを叩いたときの音


    void Start()
    {
        LinePos = this.gameObject.transform;

        //プレハブ内のAudioSourceを取得する********************************************************
        NotesSE = this.gameObject.GetComponent<AudioSource>();
        //*****************************************************************************************
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // 2D同士が接触した瞬間の１回のみ呼び出される処理
        NotesSE.PlayOneShot(HitSE);
    }
}