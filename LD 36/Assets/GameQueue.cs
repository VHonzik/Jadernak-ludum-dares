using CardGame.CardComponents;
using Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardGame
{
    public class GameQueue : MonoBehaviour
    {
        public Queue<IEnumerator> Queue { get; private set; }
        private GameManager _manager;

        
        public delegate void MinionDiedHandler(CardMinion minion, Card killer);
        // Triggered when minion dies (before the card destruction and before removing from board)
        public event MinionDiedHandler EMinionDied;

        public delegate void MinionDamagedHandler(CardMinion minion, int damage);
        // Triggered when minion is damaged by any means
        public event MinionDamagedHandler EMinionDamaged;

        public delegate void StartOfTurnHandler(bool player_turn);
        // Triggered on turn start, after card draw
        public event StartOfTurnHandler EStartOfTurn;


        public delegate void SpellCastHandler(CardSpell spell);
        // Triggered spell is cast
        public event SpellCastHandler ESpellCast;

        public delegate void TargetedSpellCastHandler(CardTargetedSpell spell);
        // Triggered when a targeted spell is cast
        public event TargetedSpellCastHandler ETargetedSpellCast;


        void Awake()
        {
            Queue = new Queue<IEnumerator>();
            _manager = GetComponent<GameManager>();

            Queue = new Queue<IEnumerator>();
            StartCoroutine(ProcessQueue());
        }

        private IEnumerator ProcessQueue()
        {
            while (true)
            {
                if (Queue.Count > 0)
                {
                    yield return StartCoroutine(Queue.Dequeue());
                }
                else
                    yield return null;
            }
        }

        public IEnumerator WaitUntilQueueEmpty()
        {
            while(Queue.Count > 0)
            {
                yield return null;
            }
        }

        private IEnumerator EvaluateDeathes()
        {
            for (int i = 0; i < _manager.Player_board.Cards.Count; i++)
            {
                Card card = _manager.Player_board.Cards[i];
                CardMinion minion = card.GetComponent<CardMinion>();
                if (minion && minion.GetComponent<CardWithHP>().Current_hp <= 0)
                {
                    Card killer = minion.KilledBy;
                    if(killer)
                    {
                        minion.KilledBy = null;
                    }
                    Queue.Enqueue(DestroyCo(minion, killer));
                }
            }

            for ( int i=0; i<_manager.Enemy_board.Cards.Count; i++)
            {
                Card card = _manager.Enemy_board.Cards[i];
                CardMinion minion = card.GetComponent<CardMinion>();
                if (minion && minion.GetComponent<CardWithHP>().Current_hp <= 0)
                {
                    Card killer = minion.KilledBy;
                    if (killer)
                    {
                        minion.KilledBy = null;
                    }
                    Queue.Enqueue(DestroyCo(minion, killer));
                }
            }

            yield break;
        }

        private IEnumerator DealDamageCo(CardMinion target, int amount)
        {
            target.ChangeHP(-amount);
            if (EMinionDamaged != null) EMinionDamaged(target, amount);
            yield break;
        }

        public void DealDamage(CardMinion target, int amount, Card source, bool evaluate_deathes)
        {
            Queue.Enqueue(DealDamageCo(target, amount));
            if (amount >= target.GetComponent<CardWithHP>().Current_hp) target.KilledBy = source;
            if (evaluate_deathes) Queue.Enqueue(EvaluateDeathes());
        }

        private IEnumerator AttackCo(CardMinion attacker, CardMinion target)
        {
            if (attacker && !attacker.IsBeingDestroyed && target && !target.IsBeingDestroyed)
            {

                AttackCapableMinion attacker_ac = attacker.GetComponent<AttackCapableMinion>();
                attacker_ac.Exhausted = attacker_ac.Exhausted + 1;
                yield return StartCoroutine(attacker_ac.AttackMovement(target));


                int target_dmg = target.GetComponent<CardWithAttack>().Current_attack;
                int attacker_dmg = attacker.GetComponent<CardWithAttack>().Current_attack;

                if (target_dmg > 0)
                {
                    if(target_dmg >= attacker.GetComponent<CardWithHP>().Current_hp)
                    {
                        attacker.KilledBy = target.GetComponent<Card>();
                    }
                    yield return StartCoroutine(DealDamageCo(attacker, target_dmg));
                }

                if (attacker_dmg > 0)
                {
                    if (attacker_dmg >= target.GetComponent<CardWithHP>().Current_hp)
                    {
                        target.KilledBy = attacker.GetComponent<Card>();
                    }
                    yield return StartCoroutine(DealDamageCo(target, attacker_dmg));
                }

                yield return StartCoroutine(EvaluateDeathes());


                if (attacker && !attacker.IsBeingDestroyed) yield return StartCoroutine(attacker_ac.AttackReturnMovement());
            }
        }

        public void Attack(CardMinion attacker, CardMinion target)
        {
            Queue.Enqueue(AttackCo(attacker, target));
        }

        private IEnumerator DestroyCo(CardMinion minion, Card killer)
        {
            if(minion)
            {
                minion.IsBeingDestroyed = true;
                Card card = minion.GetComponent<Card>();
                if(card)
                {
                    if (EMinionDied != null) EMinionDied(minion, killer);
                    if (card.Player_owned)
                    {
                        _manager.Player_board.Remove(card);
                    }
                    else
                    {
                        _manager.Enemy_board.Remove(card);
                    }

                    yield return StartCoroutine(card.Destroy());
                }

            }

        }

        public void Destroy(CardMinion minion, Card killer)
        {
            Queue.Enqueue(DestroyCo(minion, killer));
        }



        public IEnumerator PlayerDrawCardCo()
        {
            Card card = _manager.Player_stock.Draw();
            if(card)
            {
                card.GetComponent<MoveableCard>().FlipWait(0.2f).Flip(false).Move(GameManager.Player_show_card, false).MoveWait(1.0f).FlipWait(1.5f);
                _manager.Player_hand.Add(card);
                WaitForCallback<MoveableCard> helper
                    = new WaitForCallback<MoveableCard>(card.GetComponent<MoveableCard>().MovementArbitraryCoroutine);
                yield return StartCoroutine(helper.Do());

                if(_manager.Player_Turn) card.ManipulationEnable(true);
            }
        }

        public void PlayerDrawCard()
        {
            Queue.Enqueue(PlayerDrawCardCo());
        }

        public void EnemyDrawCard()
        {
            Queue.Enqueue(EnemyDrawCardCo());
        }

        private IEnumerator EnemyDrawCardCo()
        {
            Card card = _manager.Enemy_stock.Draw();
            if(card)
            {
                card.GetComponent<MoveableCard>().FlipWait(1.0f);
                _manager.Enemy_hand.Add(card);
                WaitForCallback<MoveableCard> helper
                    = new WaitForCallback<MoveableCard>(card.GetComponent<MoveableCard>().MovementArbitraryCoroutine);
                yield return StartCoroutine(helper.Do());
            }
        }

        public IEnumerator ManipulationEnabledCo(bool value)
        {
            _manager.ManipulationEnabled(value);
            yield break;
        }

        public void StartPlayerTurn()
        {
            _manager.Player_Turn = true;
            PlayerDrawCard();
            if (EStartOfTurn != null) EStartOfTurn(true);
            Queue.Enqueue(_manager.Player_Supplies.AddPernament(1));
            Queue.Enqueue(_manager.Player_Supplies.StartTurn());
            Queue.Enqueue(ManipulationEnabledCo(true));
            Queue.Enqueue(_manager.Timer.StartPlayerTurnCo());
            _manager.Player_board.OwnerTurnStart();
        }

        public IEnumerator EndPlayerTurnCo()
        {
            yield return StartCoroutine(_manager.Timer.EndTurnCo());
            _manager.Player_board.OwnerTurnEnd();
            StartEnemyTurn();
        }


        public void EndPlayerTurn()
        {
            _manager.ManipulationEnabled(false);            
            Queue.Enqueue(EndPlayerTurnCo());
        }

        private void StartEnemyTurn()
        {
            _manager.Player_Turn = false;
            EnemyDrawCard();
            if (EStartOfTurn != null) EStartOfTurn(false);
            Queue.Enqueue(_manager.Enemy_Supplies.AddPernament(1));
            Queue.Enqueue(_manager.Enemy_Supplies.StartTurn());
            _manager.Enemy_board.OwnerTurnStart();
            Queue.Enqueue(_manager.Timer.StartEnemyTurnCo());
            Queue.Enqueue(_manager.Enemy_AI.StartTurn());
        }

        public IEnumerator EndEnemyTurnCo()
        {
            yield return StartCoroutine(_manager.Timer.EndTurnCo());
            _manager.Enemy_board.OwnerTurnEnd();
            StartPlayerTurn();
        }

        public void EndEnemyTurn()
        {
            Queue.Enqueue(EndEnemyTurnCo());
        }

        public void PlayedCard(Card card)
        {
            CardWithCost cost = card.GetComponent<CardWithCost>();
            Supplies supplies = card.Player_owned ? _manager.Player_Supplies : _manager.Enemy_Supplies;
            supplies.Pay(cost.Current_cost);
        }

        public void SpellCast(CardSpell spell)
        {
            if (ESpellCast != null) ESpellCast(spell);
        }

        public void TargetedSpellCast(CardTargetedSpell spell)
        {
            if (ETargetedSpellCast != null) ETargetedSpellCast(spell);
        }

        public IEnumerator SummonMinionCo(string id, bool for_player)
        {
            Board wanted_board = for_player ? GameManager.GetInstance().Player_board : GameManager.GetInstance().Enemy_board;
            if (!wanted_board.IsFull())
            {
                Card nuke = CardAtlas.Instance.CreateCard(id);
                nuke.transform.position = GameManager.Hide_cards_position;
                nuke.Player_owned = for_player;
                yield return StartCoroutine(nuke.GetComponent<CardMinion>().DropImmidiately());
            }
        }

        public void SummonMinion(string id, bool for_player)
        {
            Queue.Enqueue(SummonMinionCo(id, for_player));
        }

        private IEnumerator GameOverEvilCo(Card evil)
        {
            _manager.InspectionEnabled(false);
            _manager.ManipulationEnabled(false);

            yield return new WaitForSeconds(2.0f);

            Card inspect = CardAtlas.Instance.CreateCard("ancientevil");
            inspect.transform.rotation = Card.Flipped_On;
            inspect.transform.position = Discover.DiscoverPosition + Vector3.up;

            yield return new WaitForSeconds(5.0f);

            GlobalObject.Instance.Won = false;

            SceneManager.LoadScene("GameOverScene");
        }

        public void GameOverEvil(Card evil)
        {
            Queue.Enqueue(GameOverEvilCo(evil));
        }
    }
}

