using CardGame.CardComponents;
using Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame
{
    public class EnemyAI : MonoBehaviour
    {
        private Queue<IEnumerator> _queue;
        private Hand _hand;
        private Supplies _supplies;
        private Board _board;

        private Board _player_board;


        void Awake()
        {
            _board = GameManager.GetInstance().Enemy_board;
            _player_board = GameManager.GetInstance().Player_board;
            _hand = GameManager.GetInstance().Enemy_hand;
            _supplies = GameManager.GetInstance().Enemy_Supplies;

            _queue = new Queue<IEnumerator>();
            StartCoroutine(ProcessQueue());
        }

        // Whenever possible summon Ancient gate accompanied with eggs
        private IEnumerator AlwaysSummonGate()
        {
            Card gate = _hand.Cards.Find(x => x.Name == "ancientgate");
            List<Card> eggs = _hand.Cards.FindAll(x => x.Name == "evilegg");

            if(gate)
            {
                int total_cost = gate.GetComponent<CardWithCost>().Current_cost;
                foreach (var egg in eggs)
                {
                    total_cost += egg.GetComponent<CardWithCost>().Current_cost;
                }

                if (total_cost <= _supplies.SuppliesAvailable() && _board.Cards.Count < Board.MaxNumberOfMinions)
                {
                    if (eggs.Count > 0)
                    {
                        yield return StartCoroutine(eggs[0].GetComponent<CardMinion>().DropAI());
                    }

                    yield return StartCoroutine(gate.GetComponent<CardMinion>().DropAI());

                    for (int i = 1; i < eggs.Count; i++)
                    {
                        yield return StartCoroutine(eggs[i].GetComponent<CardMinion>().DropAI());
                    }
                }
            }           
        }

        // Player whatever minions we have mana for
        private IEnumerator SummonMinions()
        {           
            List<Card> potential_cards = _hand.Cards.FindAll(x =>
                (x.GetComponent<CardWithCost>().CanAfford() && x.Definition.Type == CardType.Minion));
            potential_cards.Sort((x, y) => x.Definition.Cost.CompareTo(y.Definition.Cost));

            IEnumerator<Card> iterator = potential_cards.GetEnumerator();
            while (iterator.MoveNext() && iterator.Current.GetComponent<CardWithCost>().CanAfford() && !_board.IsFull())
            {
                yield return StartCoroutine(iterator.Current.GetComponent<CardMinion>().DropAI());
            }

        }

        private IEnumerator Attack()
        {

            List<Card> will_kill = new List<Card>();
            // Never attack wiht eggs
            List<Card> attack_capable_minions = _board.Cards.FindAll(x => x.GetComponent<AttackCapableMinion>().CanAttack() &&
            x.GetComponent<CardWithAttack>().Current_attack > 0 && x.Name != "evilegg");

            // Look for efficient trades
            foreach (var mine in attack_capable_minions)
            {
                // Can be killed without dying
                List<Card> targets = _player_board.Cards.FindAll(x => x.GetComponent<TargetableMinion>().CanBeTargeted
                && x.GetComponent<CardWithHP>().Current_hp <= mine.GetComponent<CardWithAttack>().Current_attack
                && x.GetComponent<CardWithAttack>().Current_attack < mine.GetComponent<CardWithHP>().Current_hp
                && !will_kill.Contains(x) );

                if(targets.Count > 0)
                {
                    // Sort by attack
                    targets.Sort((x, y) =>
                    -x.GetComponent<CardWithAttack>().Current_attack.CompareTo(y.GetComponent<CardWithAttack>().Current_attack));

                    CardMinion chosen = targets[0].GetComponent<CardMinion>();
                    GameManager.GetInstance().Game_Queue.Attack(mine.GetComponent<CardMinion>(), chosen);
                    will_kill.Add(targets[0]);
                }
            }

            yield return StartCoroutine(GameManager.GetInstance().Game_Queue.WaitUntilQueueEmpty());

            // Can we kill the biggest threats?
            List<Card> remaining_fire_power = attack_capable_minions.FindAll(x => x.GetComponent<AttackCapableMinion>().CanAttack());
            List<Card> big_threats = _player_board.Cards.FindAll(x => x.GetComponent<CardWithAttack>().Current_attack > 7);

            if(remaining_fire_power.Count > 0 && big_threats.Count > 0)
            {
                big_threats.Sort((x, y) =>
                    -x.GetComponent<CardWithAttack>().Current_attack.CompareTo(y.GetComponent<CardWithAttack>().Current_attack));
                remaining_fire_power.Sort((x, y) =>
                    x.GetComponent<CardWithAttack>().Current_attack.CompareTo(y.GetComponent<CardWithAttack>().Current_attack));
                int sum_damage = remaining_fire_power.Sum(x => x.GetComponent<CardWithAttack>().Current_attack);

                int i = 0;
                while(i < big_threats.Count)
                {
                    CardMinion threat = big_threats[i].GetComponent<CardMinion>();
                    int remaining_hp = threat.GetComponent<CardWithHP>().Current_hp;
                    if (remaining_hp <= sum_damage)
                    {
                        foreach(var minion in remaining_fire_power)
                        {
                            GameManager.GetInstance().Game_Queue.Attack(minion.GetComponent<CardMinion>(), threat);
                            remaining_hp -= minion.GetComponent<CardWithAttack>().Current_attack;

                            if (remaining_hp <= 0) break;
                        }
                        break;
                    }
                    i++;
                }
            }

            yield return StartCoroutine(GameManager.GetInstance().Game_Queue.WaitUntilQueueEmpty());

        }

        private IEnumerator EndTurn()
        {
            yield return StartCoroutine(GameManager.GetInstance().Timer.EndTurnCo());
        }

        public IEnumerator StartTurn()
        {
            yield return StartCoroutine(GameManager.GetInstance().Game_Queue.WaitUntilQueueEmpty());

            Card evil = _board.Cards.Find(x => x.GetComponent<AncientEvil>() != null);
            if (evil)
            {
                GameManager.GetInstance().Game_Queue.GameOverEvil(evil);
            }
            else
            {
                _queue.Enqueue(AlwaysSummonGate());
                _queue.Enqueue(SummonMinions());
                _queue.Enqueue(Attack());
                _queue.Enqueue(EndTurn());
            }

            yield break;
        }

        private IEnumerator ProcessQueue()
        {
            while (true)
            {
                if (_queue.Count > 0)
                {
                    yield return StartCoroutine(_queue.Dequeue());
                }
                else
                    yield return null;
            }
        }
    }
}
