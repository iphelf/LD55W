using Roulette.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roulette.Scripts.ViewCtrls
{
    public class CardRow : MonoBehaviour
    {
        //[SerializeField] private Vector3[] CardsPosition;
        List<ItemType> Cards = new List<ItemType>();

        [SerializeField] private GameObject newCard;
        private Vector3 newCardPrePosition;
        private Vector3 newCardDrawPosition;//最后位置
        // Start is called before the first frame update
        void Start()
        {
            newCardPrePosition = newCard.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
        }
        private void ShowNewCard(ItemType card, int CardIndex)
        {
            //ʵ��������
            //GameObject newCard = Instantiate(Card, CardsPosition[CardIndex], Quaternion.identity);
            //newCard.transform.SetParent(transform);
            //����active����
            transform.GetChild(CardIndex).gameObject.GetComponent<CardCtrl>().card = card;
            transform.GetChild(CardIndex).gameObject.SetActive(true);
        }

        private void ResumeNewCard()
        {
            newCard.SetActive(false);
            newCard.transform.position = newCardPrePosition;
        }

        public void DrawCardFromDeck(ItemType card)
        {
            newCard.GetComponent<CardCtrl>().card = card;
            newCard.SetActive(true);
            
        }

        public void AppendCard(ItemType card)
        {
            ResumeNewCard();
            Cards.Add(card);
            ShowNewCard(card, Cards.Count()-1);
        }
        public void RegretfullyDisposeLastDrawnCard()
        {
            ResumeNewCard();
        }


    }
}