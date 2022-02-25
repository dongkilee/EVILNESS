using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SoundManager : Singleton<SoundManager>
{
    static public float SoundVolume;
    static public float BGMVolume;

    public float LowPitchange = 0.95f;
    public float HighPitchRange = 1.05f;

    public AudioSource BGM; //배경음
    public AudioSource SFXSource; //효과음

    // BGM
    public AudioClip StageBGM;
    public AudioClip BossBGM;
    public AudioClip StageRainBGM;

    //막타 기합소리
    public AudioClip[] shout;

    // 검소리
    public AudioClip slash_1;
    public AudioClip slash_2;
    public AudioClip slash_3;
    public AudioClip slash_4;

    // 검으로 후들겨 맞는 소리
    public AudioClip attack1_hit;
    public AudioClip attack2_hit;
    public AudioClip attack3_hit;
    public AudioClip attack4_hit;

    // 몬스터 죽었을 때
    public AudioClip Monster_die;

    // 플레이어 발자국 소리
    public AudioClip[] WaterWalk;
    // 플레이어 발자국 소리 보스방
    public AudioClip[] BossRoomWalk;

    // 플레이어 대쉬
    public AudioClip Dash;

    // 보스 스킬 사운드
    public AudioClip[] BossSkill;
    // 보스 미사일 폭발음
    public AudioClip BossMisilleBoom;
    // 보스 오라 이펙트 사운드
    public AudioClip Aurora;

    // UI버튼 사운드
    public AudioClip ButtonClick;
    public AudioClip ButtonUp;

    // 쥐 공격
    public AudioClip[] Rat_Attack;
    // 히드라 공격
    public AudioClip Hydra_Attack;


    public void Start()
    {
        SoundVolume = PlayerPrefs.GetFloat("SoundVolume", 1);
        BGMVolume = PlayerPrefs.GetFloat("BGMVoume", 1);

        PlayBGM(GetComponent<AudioSource>(), StageBGM, true, BGMVolume);
        
    }

    public void PlaySingle(AudioSource source ,AudioClip clip)
    {
        SFXSource = source;
        SFXSource.clip = clip;
        SFXSource.PlayOneShot(clip, SoundVolume);
    }

    public void PlayBGM(AudioSource source, AudioClip clip, bool loop, float _sound)      // BGM 루프
    {
        
        BGM = source;
        BGM.clip = clip;
        BGM.loop = loop;
        BGM.volume = _sound;
        BGM.Play();
    }

    public void RandomizeSFX(AudioSource source, params AudioClip[] clips)          // 랜덤으로 사운드 재생
    {
        SFXSource = source;
        int randomIndex = UnityEngine.Random.Range(0, clips.Length);
        float randomPitch = UnityEngine.Random.Range(LowPitchange, HighPitchRange);
        SFXSource.pitch = randomPitch;
        SFXSource.clip = clips[randomIndex];
        SFXSource.PlayOneShot(clips[randomIndex], SoundVolume);
    }

    static int rand = 0;

    public void PlayerRandomizeSFX(AudioSource source, params AudioClip[] clips)          // 랜덤으로 사운드 재생
    {
        int randomIndex;
        SFXSource = source;
        while (true)
        {
            randomIndex = UnityEngine.Random.Range(0, clips.Length);
            if(randomIndex != rand)
            {
                rand = randomIndex;
                break;
            }
        }
        float randomPitch = UnityEngine.Random.Range(LowPitchange, HighPitchRange);
        SFXSource.pitch = randomPitch;
        SFXSource.clip = clips[randomIndex];
        SFXSource.PlayOneShot(clips[randomIndex], SoundVolume);
    }

}
