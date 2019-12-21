using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;//곡의 이름
    public AudioClip clip;//곡
}

public class SoundManager : MonoBehaviour
{
    //다른scene이동해도 사라지지 않는 싱글턴으로 만들것

    static public SoundManager instance;

    #region singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);//다른 scene으로 이동해도 사라지지 않게 
        }   
        else
            Destroy(this.gameObject);
        //다른scene에서 이것이 있는 scene으로 이동했을때 또 생기므로 기존의것 지운다 
    }
    #endregion singleton

    public AudioSource[] audioSourceEffects;//현재 재생할 효과음-여러개일수 있다
    public AudioSource audioSourceBgm;  //현재 재생할 bgm

    public string[] playSoundName;

    public Sound[] effectSounds;        //효과음 담아두는 곳
    public Sound[] bgmSounds;           //bgm담아두는곳


    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
    }

    //재생했으면 하는 효과음을 이름으로 찾아서 재생시키기
    public void PlaySE(string _name)
    {
        for(int i = 0; i < effectSounds.Length; i++)
        {
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)//현재 재생중이 아니라면
                    {
                        playSoundName[j] = effectSounds[i].name;//재생중인 audioSourceEffect와 순서를 일치시키고자 j에 넣는다
                        audioSourceEffects[j].clip = effectSounds[i].clip;//현재 재생 목록에 넣고 재생
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용AudioSource가 사용중입니다");//j크기내에서 audioSourceEffects에 바꿀수 있는 공간이 없는것
                return;//->audioSourceEffect의 크기를 늘릴것
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다");
    }

    //모든 효과음 재생 중단
    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    //특정 효과음 재생 중단
    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
        Debug.Log("재생중인" + _name + "사운드가 없습니다");
    }




    //////////////////////////////////////////////////bgm영역
}
