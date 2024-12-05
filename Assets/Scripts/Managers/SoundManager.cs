using GameSystem;
using SubWeapon;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Sound
{
    public class SoundManager : Singleton<SoundManager>
    {

        // === AudioSource ===

        private AudioSource bgmSource;
        private AudioSource[] seSource = new AudioSource[5];
        private AudioSource[] voiceSource = new AudioSource[5];
        private AudioSource[] enemySE = new AudioSource[15];
        private AudioSource[] subWeaponSE = new AudioSource[15];
        private AudioSource[] buttonSE = new AudioSource[5];
        private AudioSource[] skillSE = new AudioSource[16];

        // === AudioClip ===

        [SerializeField]
        private AudioClip[] BGM;
        [SerializeField]
        private AudioClip[] SE;
        [SerializeField]
        private AudioClip[] Voice;
        [SerializeField]
        private AudioClip[] enemyAudio;
        [SerializeField]
        private AudioClip[] subWeaponAudio;
        [SerializeField]
        private AudioClip[] buttonAudio;
        [SerializeField]
        private AudioClip[] skillAudio;

        SceneController.GameScene currentGameScene;

        private List<int> deathEnemyList = new List<int>();

        private float seVolume;
        private float voiceVolume;
        public enum BGMID
        {
            title,
            stageSelect,
            battle,
            gameClear,
            gameOver
        }

        public enum SEID
        {
            levelUP,
            healItem,
            skillItem,
            coin
        }

        public enum VoideID
        {
            aka,
        }

        public enum enemyAudioID
        {
            enemy,
            midbossEnemy,
            bossEnemy
        }

        public enum subWeaponAudioID
        {
            attackField,
            lightningStrike,
            fireAttack,
            missile,
            ball,
            boomerang,
            funnel,
            landmine,
            shield,
            gravitationalField,
            fastFire
        }

        public enum buttonAudioID
        {
            startButton,
            selectButton,
            pauseButton,
            skillButton,
            resultButton,
        }

        public enum skillAudioID
        {
            skill,
            skillEnd,
            wink
        }

        private void Awake()
        {
            Init();
            StartCoroutine(InitAudioSoruce());
            seVolume = 1.0f;
            voiceVolume = 1.0f;
        }

        private IEnumerator InitAudioSoruce()
        {

            // BGM AudioSource
            bgmSource = gameObject.AddComponent<AudioSource>();
            // BGM�̓��[�v��L���ɂ���
            bgmSource.loop = true;

            // SE AudioSource
            for (int i = 0; i < seSource.Length; i++)
            {
                seSource[i] = gameObject.AddComponent<AudioSource>();

                if (i % 10 == 0)
                    yield return null;
            }

            for (int i = 0; i < voiceSource.Length; i++)
            {
                voiceSource[i] = gameObject.AddComponent<AudioSource>();
            }

            // EnemySE AudioSource
            for (int i = 0; i < enemySE.Length; i++)
            {
                enemySE[i] = gameObject.AddComponent<AudioSource>();

                if (i % 10 == 0)
                    yield return null;
            }


            // EnemySE AudioSource
            for (int i = 0; i < subWeaponSE.Length; i++)
            {
                subWeaponSE[i] = gameObject.AddComponent<AudioSource>();

                if (i % 10 == 0)
                    yield return null;
            }

            for (int i = 0; i < buttonSE.Length; i++)
            {
                buttonSE[i] = gameObject.AddComponent<AudioSource>();
            }

            for (int i = 0; i < skillSE.Length; i++)
            {
                skillSE[i] = gameObject.AddComponent<AudioSource>();
            }

            // �V�[�������擾���ď�����
            PlayBGMScene(GameManager.I.sceneController.GetGameScene());

            yield break;
        }

        private void Update()
        {
            if (deathEnemyList.Count != 0)
                PlayDeathEnemySE();

            // �V�[���ύX���`�F�b�N
            var newGameScene = GameManager.I.sceneController.GetGameScene();
            if (newGameScene != currentGameScene)
            {
                currentGameScene = newGameScene;
                PlayBGMScene(currentGameScene);
            }
           

        }

        /// <summary>
        /// scene��BGM���Đ�����
        /// </summary>
        /// <param name="gameScene">scene�̎��</param>
        private void PlayBGMScene(SceneController.GameScene gameScene)
        {
            switch (gameScene)
            {
                case SceneController.GameScene.title:
                    PlayBGM(BGMID.title);
                    break;
                case SceneController.GameScene.stageSelect:
                    //�X�e�[�W�I����BGM
                    // PlayBGM(0);
                    break;
                case SceneController.GameScene.battle:
                    PlayBGM(BGMID.battle);
                    break;
                case SceneController.GameScene.gameClear:
                    PlayBGM(BGMID.gameClear);
                    break;
                case SceneController.GameScene.gameOver:
                    PlayBGM(BGMID.gameOver);
                    break;
                default:
                    StopBGM(); // �f�t�H���g��BGM���~
                    break;
            }
        }

        // ���ʐݒ���O������Ăяo����悤�ɂ���
        /// <summary>
        /// BGM�{�����[���擾�p
        /// </summary>
        /// <param name="volume">BGM�̉���</param>
        public void SetBGMVolume(float volume)
        {
            bgmSource.volume = volume;
        }
        /// <summary>
        /// SE�{�����[���擾�p
        /// </summary>
        /// <param name="volume">SE�̉���</param>
        public void SetSEVolume(float volume)
        {
            seVolume = volume;
        }

        public void SetVoiceVolume(float volume)
        {
            voiceVolume = volume;
        }

        /// <summary>
        /// BGM�Đ�
        /// </summary>
        /// <param name="bgmID">�Đ�����BGM�̎���</param>
        private void PlayBGM(BGMID bgmID)
        {
            if (0 > bgmID || BGM.Length <= (int)bgmID)
            {
                return;
            }
            // ����BGM�̏ꍇ�͉������Ȃ�
            if (bgmSource.clip == BGM[(int)bgmID])
            {
                return;
            }
            bgmSource.Stop();
            bgmSource.clip = BGM[(int)bgmID];
            bgmSource.Play();
        }

        /// <summary>
        /// ���ׂĂ�BGM��~
        /// </summary>
        public void StopBGM()
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }


        /// <summary>
        /// SE�Đ�
        /// </summary>
        /// <param name="seID">�Đ�����SE�̎���</param>
        public void PlaySE(SEID seID)
        {
            // �Đ����Ŗ���AudioSouce�Ŗ炷
            foreach (AudioSource source in seSource)
            {
                if (false == source.isPlaying)
                {
                    source.clip = SE[(int)seID];
                    source.volume = seVolume;
                    source.Play();
                    return;
                }
            }
        }

        /// <summary>
        /// ���ׂĂ�SE��~
        /// </summary>
        public void DestroyButtleSE()
        {
            // �S�Ă�SE�p��AudioSouce���~����
            foreach (AudioSource source in seSource)
            {
                source.Stop();
                source.clip = null;
            }

            foreach (AudioSource source in enemySE)
            {
                source.Stop();
                source.clip = null;
            }

            foreach (AudioSource source in subWeaponSE)
            {
                source.Stop();
                source.clip = null;
            }

            foreach (AudioSource source in voiceSource)
            {
                source.Stop();
                source.clip = null;
            }

        }

        /// <summary>
        /// SE�Đ�
        /// </summary>
        /// <param name="seID">�Đ�����SE�̎���</param>
        public void PlayVoiceSE(VoideID voiceID)
        {
            DestroyButtleSE();
            // �Đ����Ŗ���AudioSouce�Ŗ炷
            foreach (AudioSource source in voiceSource)
            {
                if (false == source.isPlaying)
                {
                    source.clip = Voice[(int)voiceID];
                    source.volume = voiceVolume;
                    source.Play();
                    return;
                }
            }
        }

        public void StopVoice()
        {
            // �S�Ă�SE�p��AudioSouce���~����
            foreach (AudioSource source in voiceSource)
            {
                source.Stop();
                source.clip = null;
            }
        }

        /// <summary>
        /// ���ׂĂ�SE����~����
        /// </summary>
        public void PauseStopSE()
        {
            // �eSE AudioSource���ꎞ��~
            foreach (AudioSource source in seSource)
            {
                if (source.isPlaying)
                {
                    source.Pause();
                }
            }


            foreach (AudioSource source in enemySE)
            {
                if (source.isPlaying)
                {
                    source.Pause();
                }
            }

            foreach (AudioSource source in subWeaponSE)
            {
                if (source.isPlaying)
                {
                    source.loop = false;
                    source.Pause();
                }
            }

            foreach(AudioSource source in voiceSource)
            {
                if (source.isPlaying)
                {
                    source.Pause();
                }
            }
        }
        /// <summary>
        /// ��~����SE���Đ������
        /// </summary>
        public void ResumeSE()
        {
            foreach (AudioSource source in seSource)
            {
                if (source.clip != null && !source.isPlaying)
                {
                    source.UnPause();
                }
            }

            foreach (AudioSource source in enemySE)
            {
                if (source.clip != null && !source.isPlaying)
                {
                    source.UnPause();
                }
            }

            foreach (AudioSource source in subWeaponSE)
            {
                if (source.clip != null && !source.isPlaying)
                {
                    source.volume = seVolume;
                    source.loop = true;
                    source.UnPause();

                }
            }

            foreach (AudioSource source in voiceSource)
            {
                if (source.clip != null && !source.isPlaying)
                {
                    source.UnPause();
                }
            }
        }

        /// <summary>
        /// �G�����񂾂Ƃ���SE���Đ�
        /// </summary>
        /// <param name="enemyID">�G�l�~�[�̎���</param>
        public void PlayDeathEnemySE(Enemy.EnemyManager.EnemyCharactersID enemyID)
        {
            deathEnemyList.Add((int)enemyID);
        }

        private void PlayDeathEnemySE()
        {
            var playCnt = 0;
            var playList = new List<AudioSource>();


            foreach (var deathEnemy in deathEnemyList)
            {
                foreach (AudioSource source in enemySE)
                {
                    if (!source.isPlaying)
                    {
                        source.clip = enemyAudio[deathEnemy];
                        source.Play();
                        playCnt++;
                        playList.Add(source);

                        break;
                    }
                }
            }

            var volume = seVolume / (float)playCnt;
            if (seVolume == 0.0f)
                volume = 0.0f;

            foreach (var audio in playList)
            {
                audio.volume = volume;
            }

            deathEnemyList.Clear();
        }
        public void PlayButtonSE(buttonAudioID buttonID)
        {

            foreach (AudioSource source in buttonSE)
            {
                if (false == source.isPlaying)
                {
                    source.clip = buttonAudio[(int)buttonID];
                    source.volume = seVolume;
                    source.Play();
                    return;
                }
            }
        }

        public void PlaySkillSE(skillAudioID skillID)
        {

            foreach (AudioSource source in skillSE)
            {
                if (false == source.isPlaying)
                {
                    source.clip = skillAudio[(int)skillID];
                    source.volume = seVolume;
                    source.Play();
                    return;
                }
            }
        }

        /// <summary>
        /// �����SE�Đ�
        /// </summary>
        /// <param name="weaponID">�Đ����镐��SE�̎��</param>
        /// <param name="loop"></param>
        public void PlaySubWeaponSE(subWeaponAudioID weaponID, bool loop)
        {

            foreach (AudioSource source in subWeaponSE)
            {
                if (!source.isPlaying)
                {

                    source.clip = subWeaponAudio[(int)weaponID];
                    source.volume = seVolume;
                    source.loop = loop;
                    source.spatialBlend = 1.0f; // 3D����
                    source.maxDistance = 11.0f;
                    source.minDistance = 5.0f;
                    source.transform.position = Player.PlayerManager.I.GetPlayerTransform().position;
                    source.Play();
                   
                    return;
                }
            }
        }

        /// <summary>
        /// �����SE��~
        /// </summary>
        /// <param name="weaponID">��~���镐��SE�̎��</param>
        public void StopSubWeaponSE(subWeaponAudioID weaponID)
        {
            // �e����Ɋ֘A�t����ꂽ�����~
            foreach (AudioSource source in subWeaponSE)
            {
                if (source.isPlaying)
                {
                    if (source.clip == subWeaponAudio[(int)weaponID])
                    {
                        source.Stop();
                        source.clip = null;
                        return;
                    }
                }
            }
        }

       
     }
 }








