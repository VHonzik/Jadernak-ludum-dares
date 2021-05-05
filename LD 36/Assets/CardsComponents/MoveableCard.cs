using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card), typeof(DetachableCard))]
    class MoveableCard : MonoBehaviour
    {
        private static float MaxSpeed = 7.0f;
        private static float MinSpeed = 3.0f;

        private Queue<IEnumerator> _movement_queue;
        private Queue<IEnumerator> _rotation_queue;

        private HighlightableCard _higlight_card;
        private DetachableCard _detach_card;
        private Card _card;

        public bool Is_moving { get; set; }

        void Awake()
        {    
            _movement_queue = new Queue<IEnumerator>();
            StartCoroutine(ProcessMovementQueue());

            _rotation_queue = new Queue<IEnumerator>();
            StartCoroutine(ProcessFlipQueue());

            Is_moving = false;

            _higlight_card = GetComponent<HighlightableCard>();
            _card = GetComponent<Card>();
            _detach_card = GetComponent<DetachableCard>();
        }

        public bool IsReadyForMovement()
        {
            return !Is_moving && _movement_queue.Count <= 0;
        }

        private IEnumerator ProcessMovementQueue()
        {
            while (true)
            {
                if (_movement_queue.Count > 0)
                {
                    yield return StartCoroutine(_movement_queue.Dequeue());
                }
                else
                    yield return null;
            }
        }

        private IEnumerator ProcessFlipQueue()
        {
            while (true)
            {
                if (_rotation_queue.Count > 0)
                    yield return StartCoroutine(_rotation_queue.Dequeue());
                else
                    yield return null;
            }
        }

        public MoveableCard Flip(bool instant)
        {
            _rotation_queue.Enqueue(instant ? FlipInstantly() : Flip());
            return this;
        }

        public MoveableCard Move(Vector3 wanted_pos, bool instant)
        {
            if (_higlight_card && _higlight_card.Highlighted) _higlight_card.Highlight(false);

            _movement_queue.Enqueue(instant ? MoveToInstantly(wanted_pos) : MoveTo(wanted_pos));
            return this;
        }

        public MoveableCard MoveDetached(Vector3 wanted_pos)
        {
            _movement_queue.Enqueue(MoveToDetached(wanted_pos));
            return this;
        }


        public MoveableCard ShakeDetached()
        {
            _movement_queue.Enqueue(Shake());
            return this;
        }

        public MoveableCard ReturnDetached()
        {
            _movement_queue.Enqueue(MoveToDetached(transform.position));
            return this;
        }

        public MoveableCard Rotate(Quaternion wanted_rot, bool instant)
        {
            _rotation_queue.Enqueue(instant ? RotateInstantly(wanted_rot) : Rotate(wanted_rot));
            return this;
        }

        public MoveableCard MoveWait(float seconds)
        {
            _movement_queue.Enqueue(Wait(seconds));
            return this;
        }

        public MoveableCard FlipWait(float seconds)
        {
            _rotation_queue.Enqueue(Wait(seconds));
            return this;
        }

        public MoveableCard SetIsInHandCoroutine(bool value)
        {
            _movement_queue.Enqueue(_card.SetIsInHandWrapper(value));
            return this;
        }

        public MoveableCard MovementArbitraryCoroutine(IEnumerator coroutine)
        {
            _movement_queue.Enqueue(coroutine);
            return this;
        }


        static public IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        public IEnumerator MoveToDetached(Vector3 wanted_pos)
        {
            Is_moving = true;

            Vector3 _previous_position = _detach_card.Detached_position;
            Vector3 _middle = Vector3.Lerp(wanted_pos, _previous_position, 0.5f);
            float _distance = Vector3.Distance(_previous_position, wanted_pos);

            if (_distance < MinSpeed)
            {
                float _t = 0.0f;
                while (true)
                {
                    if (_t >= 1.0f) break;

                    _detach_card.Detached_position = Vector3.Lerp(_previous_position, wanted_pos, _t);
                    _t += Time.deltaTime * 2.0f;

                    yield return null;
                }

            }
            else
            {
                float _speed = 0.0f;

                while (true)
                {
                    if (wanted_pos == _detach_card.Detached_position) break;

                    float t = 1.0f - (Vector3.Distance(_detach_card.Detached_position, _middle) /
                        (Vector3.Distance(wanted_pos, _previous_position) * 0.5f));
                    _speed = Mathf.Lerp(MinSpeed, MaxSpeed, t);
                    _detach_card.Detached_position = Vector3.MoveTowards(_detach_card.Detached_position,
                        wanted_pos, _speed * Time.deltaTime);

                    yield return null;
                }
            }

            Is_moving = false;
        }

        public IEnumerator MoveTo(Vector3 wanted_pos)
        {
            Is_moving = true;

            Vector3 _previous_position = transform.position;
            Vector3 _middle = Vector3.Lerp(wanted_pos, _previous_position, 0.5f);
            float _distance = Vector3.Distance(_previous_position, wanted_pos);

            if (_distance < MinSpeed)
            {
                float _t = 0.0f;
                while (true)
                {
                    if (_t >= 1.0f) break;

                    transform.position = Vector3.Lerp(_previous_position, wanted_pos, _t);
                    _t += Time.deltaTime * 2.0f;

                    yield return null;
                }

            }
            else
            {
                float _speed = 0.0f;

                while (true)
                {
                    if (wanted_pos == transform.position) break;

                    float t = 1.0f - (Vector3.Distance(transform.position, _middle) /
                        (Vector3.Distance(wanted_pos, _previous_position) * 0.5f));
                    _speed = Mathf.Lerp(MinSpeed, MaxSpeed, t);
                    transform.position = Vector3.MoveTowards(transform.position,
                        wanted_pos, _speed * Time.deltaTime);

                    yield return null;
                }
            }

            Is_moving = false;

        }

        public IEnumerator MoveToInstantly(Vector3 wanted_pos)
        {
            transform.position = wanted_pos;
            yield break;
        }

        public IEnumerator Flip()
        {
            Is_moving = true;

            float _t = 0.0f;

            float difference = Quaternion.Angle(transform.rotation, Card.Flipped_On);
            bool _initialy_flipped_on = difference < 5.0f;

            Quaternion _initial_state = _initialy_flipped_on ? Card.Flipped_On : Card.Flipped_Off;
            Quaternion _wanted_state = !_initialy_flipped_on ? Card.Flipped_On : Card.Flipped_Off;

            while (true)
            {
                if (_t >= 1.0f) break;

                transform.rotation =
    Quaternion.Lerp(_initial_state, _wanted_state, _t);
                _t += Time.deltaTime;

                yield return null;
            }

            Is_moving = false;
        }


        public IEnumerator FlipInstantly()
        {
            transform.rotation = Quaternion.Angle(transform.rotation, Card.Flipped_On) < 5.0f ? Card.Flipped_Off : Card.Flipped_On;
            yield break;
        }

        public IEnumerator Rotate(Quaternion wanted_rot)
        {
            Is_moving = true;

            float _t = 0.0f;

            Quaternion _initial_state = transform.rotation;
            Quaternion _wanted_state = wanted_rot;

            while (true)
            {
                if (_t >= 1.0f) break;

                transform.rotation = Quaternion.Lerp(_initial_state, _wanted_state, _t);
                _t += 3.0f * Time.deltaTime;

                yield return null;
            }

            Is_moving = false;
        }

        public IEnumerator RotateInstantly(Quaternion wanted_rot)
        {
            transform.rotation = wanted_rot;
            yield break;
        }

        public IEnumerator Shake()
        {
            yield break;
        }
    }
}
