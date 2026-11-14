using System;
using UnityEngine;

namespace Afterlight
{
    public sealed class MissionManager : MonoBehaviour
    {
        private const string ClueKey = "AFTERLIGHT_CLUE_MASK_V1";
        private const string ChapterKey = "AFTERLIGHT_CHAPTER_ONE_V1";
        private const int AllClues = 0b1111;

        public event Action OnChanged;

        public int ClueCount { get; private set; }
        public bool ChapterComplete { get; private set; }
        public int ClueMask { get; private set; }

        public string Objective
        {
            get
            {
                if (ChapterComplete) return "FOLLOW THE SIGNAL BEYOND NAMBARA";
                if (ClueCount >= 4) return "REACH THE LIGHTHOUSE";
                return "FIND THE FOUR CLUES IN NAMBARA";
            }
        }

        private void Awake()
        {
            Load();
        }

        public bool HasClue(int index)
        {
            return index >= 0 && index < 4 && (ClueMask & (1 << index)) != 0;
        }

        public bool CollectClue(int index)
        {
            if (index < 0 || index > 3 || HasClue(index)) return false;
            ClueMask |= 1 << index;
            Recount();
            Save();
            OnChanged?.Invoke();
            return true;
        }

        public bool TryCompleteChapter()
        {
            if (ClueCount < 4 || ChapterComplete) return false;
            ChapterComplete = true;
            Save();
            OnChanged?.Invoke();
            return true;
        }

        public void ResetProgress()
        {
            ClueMask = 0;
            ClueCount = 0;
            ChapterComplete = false;
            Save();
            OnChanged?.Invoke();
        }

        private void Recount()
        {
            int count = 0;
            for (int i = 0; i < 4; i++)
                if ((ClueMask & (1 << i)) != 0) count++;
            ClueCount = count;
        }

        private void Load()
        {
            ClueMask = PlayerPrefs.GetInt(ClueKey, 0) & AllClues;
            ChapterComplete = PlayerPrefs.GetInt(ChapterKey, 0) == 1;
            Recount();
            if (ChapterComplete && ClueMask != AllClues) ChapterComplete = false;
        }

        private void Save()
        {
            PlayerPrefs.SetInt(ClueKey, ClueMask);
            PlayerPrefs.SetInt(ChapterKey, ChapterComplete ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
