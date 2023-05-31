namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Spell : MonoBehaviour
    {

        [SerializeField] public GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;

        [SerializeField] private UI_SpellBrewing _brewPrefab = null;
        [SerializeField] private RectTransform _brewGrid = null;

        private static UI_Spell _instance = null; public static UI_Spell instanse { get { return _instance; } }

        private List<UI_SpellBrewing> brewingItems = new List<UI_SpellBrewing>();
        [SerializeField] private List<UI_SpellItem> uiSpells = new List<UI_SpellItem>();

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        [HideInInspector] public bool isOpen = false;

        private void ClearBrewingItems()
        {
            for (int i = 0; i < brewingItems.Count; i++)
            {
                if (brewingItems[i])
                {
                    Destroy(brewingItems[i].gameObject);
                }
            }
            brewingItems.Clear();
        }

        public void Initialize()
        {
            for (int i = 0; i < uiSpells.Count; i++)
            {
                for (int j = 0; j < Player.instanse.initializationData.serverSpells.Count; j++)
                {
                    if (uiSpells[i].id == Player.instanse.initializationData.serverSpells[j].id)
                    {
                        uiSpells[i].Initialize(Player.instanse.initializationData.serverSpells[j]);
                        break;
                    }
                }
            }
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
        }

        public void SetStatus(bool status)
        {
            ClearBrewingItems();
            if (status)
            {
                UI_Main.instanse.SetStatus(false);
                Initialize();
                Sync();
            }
            _elements.SetActive(status);
            isOpen = status;
        }

        private void UpdateBrewingList()
        {
            for (int i = 0; i < Player.instanse.data.spells.Count; i++)
            {
                if (Player.instanse.data.spells[i].ready == false)
                {
                    int x = -1;
                    for (int j = 0; j < brewingItems.Count; j++)
                    {
                        if (brewingItems[j] && Player.instanse.data.spells[i].databaseID == brewingItems[j].databaseID)
                        {
                            x = j;
                            break;
                        }
                    }
                    if (x >= 0)
                    {

                    }
                    else
                    {
                        UI_SpellBrewing spell = Instantiate(_brewPrefab, _brewGrid.transform);
                        spell.Initialize(Player.instanse.data.spells[i]);
                        brewingItems.Add(spell);
                    }
                }
            }
            ResetBrewingItemsIndex();
        }

        private void Close()
        {
            SetStatus(false);
            UI_Main.instanse.SetStatus(true);
        }

        public void Sync()
        {
            for (int i = 0; i < uiSpells.Count; i++)
            {
                uiSpells[i].Sync();
            }

            for (int i = brewingItems.Count - 1; i >= 0; i--)
            {
                if (brewingItems[i])
                {
                    int x = -1;
                    for (int j = 0; j < Player.instanse.data.spells.Count; j++)
                    {
                        if (Player.instanse.data.spells[j].databaseID == brewingItems[i].databaseID)
                        {
                            x = j;
                            break;
                        }
                    }
                    if (x >= 0)
                    {
                        if (Player.instanse.data.spells[x].ready)
                        {
                            RemoveTrainingItem(i);
                        }
                    }
                    else
                    {
                        RemoveTrainingItem(i);
                    }
                }
                else
                {
                    RemoveTrainingItem(i);
                }
            }
            UpdateBrewingList();
        }

        public void RemoveTrainingItem(int i)
        {
            if (i < 0 || i >= brewingItems.Count)
            {
                return;
            }
            if (brewingItems[i])
            {
                Destroy(brewingItems[i].gameObject);
            }
            brewingItems.RemoveAt(i);
            ResetBrewingItemsIndex();
        }

        public void ResetBrewingItemsIndex()
        {
            for (int j = 0; j < brewingItems.Count; j++)
            {
                if (brewingItems[j])
                {
                    brewingItems[j].index = j;
                }
            }
        }

    }
}