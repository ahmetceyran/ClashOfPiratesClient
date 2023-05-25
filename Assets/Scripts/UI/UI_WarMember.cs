namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UI_WarMember : MonoBehaviour
    {

        [SerializeField] private Image _image = null;
        [SerializeField] private TextMeshProUGUI _name = null;
        public GameObject selectedEffects = null;
        [SerializeField] private GameObject bestAttack = null;
        [SerializeField] private GameObject bestAttackStar1 = null;
        [SerializeField] private GameObject bestAttackStar2 = null;
        [SerializeField] private GameObject bestAttackStar3 = null;
        [SerializeField] private GameObject attack1 = null;
        [SerializeField] private GameObject attack2 = null;

        [HideInInspector] public Data.ClanMember _data = null;

        [HideInInspector] public int remainedAttacks = 0;

        private void Start()
        {
            Button button = GetComponentInChildren<Button>();
            if (button)
            {
                button.onClick.AddListener(Select);
            }
        }

        public void Initialize(Data.ClanMember data, int attacksDone, Data.ClanWarAttack bestEnemyAttack)
        {
            remainedAttacks = Data.clanWarAttacksPerPlayer - attacksDone;
            selectedEffects.SetActive(false);
            _data = data;
            _name.text = (_data.warPos + 1).ToString() + ". " + data.name;

            attack1.SetActive(remainedAttacks > 0);
            attack2.SetActive(remainedAttacks > 1);

            if (bestEnemyAttack != null)
            {
                bestAttackStar1.SetActive(bestEnemyAttack.stars > 0);
                bestAttackStar2.SetActive(bestEnemyAttack.stars > 1);
                bestAttackStar3.SetActive(bestEnemyAttack.stars > 2);
                bestAttack.SetActive(true);
            }
            else
            {
                bestAttack.SetActive(false);
            }
        }

        private void Select()
        {
            UI_Clan.instanse.SelectWarMember(this);
        }

    }
}