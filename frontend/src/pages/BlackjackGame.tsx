import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { BlackjackProfile, Card, Suit, Rank } from '../types/blackjack';
import { getProfile, addChips, discardChips } from '../api/blackjackApi';

type GameStatus = 'loading' | 'betting' | 'playing' | 'finished';
type GameResult = 'win' | 'lose' | 'push' | null;

const SUITS: Suit[] = ['♠', '♥', '♦', '♣'];
const RANKS: Rank[] = ['A', '2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K'];

function buildDeck(): Card[] {
  const deck: Card[] = [];
  for (const suit of SUITS)
    for (const rank of RANKS)
      deck.push({ suit, rank });
  for (let i = deck.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [deck[i], deck[j]] = [deck[j], deck[i]];
  }
  return deck;
}

function cardNumericValue(rank: Rank): number {
  if (rank === 'A') return 11;
  if (['J', 'Q', 'K'].includes(rank)) return 10;
  return parseInt(rank);
}

function handValue(cards: Card[]): number {
  const visible = cards.filter(c => !c.faceDown);
  let value = visible.reduce((sum, c) => sum + cardNumericValue(c.rank), 0);
  let aces = visible.filter(c => c.rank === 'A').length;
  while (value > 21 && aces > 0) { value -= 10; aces--; }
  return value;
}

function isRed(suit: Suit): boolean {
  return suit === '♥' || suit === '♦';
}

export default function BlackjackGame() {
  const navigate = useNavigate();
  const [status, setStatus] = useState<GameStatus>('loading');
  const [profile, setProfile] = useState<BlackjackProfile | null>(null);
  const [deck, setDeck] = useState<Card[]>([]);
  const [playerCards, setPlayerCards] = useState<Card[]>([]);
  const [dealerCards, setDealerCards] = useState<Card[]>([]);
  const [bet, setBet] = useState(10);
  const [betInput, setBetInput] = useState('10');
  const [result, setResult] = useState<GameResult>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    openBlackJack();
  }, []);

  async function openBlackJack() {
    try {
      const p = await getProfile();
      setProfile(p);
      setStatus('betting');
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load profile.');
    }
  }

  function selectBet(amount: number) {
    const clamped = Math.min(amount, profile?.chips ?? amount);
    setBet(clamped);
    setBetInput(String(clamped));
  }

  function startGame() {
    if (!profile || bet < 1 || bet > profile.chips) return;

    const newDeck = buildDeck();
    const d1 = newDeck.pop()!;
    const p1 = newDeck.pop()!;
    const d2 = { ...newDeck.pop()!, faceDown: true };
    const p2 = newDeck.pop()!;

    const pCards = [p1, p2];
    const dCards = [d1, d2];

    setDeck(newDeck);
    setPlayerCards(pCards);
    setDealerCards(dCards);

    if (handValue(pCards) === 21) {
      const revealed = dCards.map(c => ({ ...c, faceDown: false }));
      setDealerCards(revealed);
      finishGame(pCards, revealed, bet);
    } else {
      setStatus('playing');
    }
  }

  function playDealerTurn(currentDeck: Card[], currentDealerCards: Card[]) {
    const nextDeck = [...currentDeck];
    let dCards = currentDealerCards.map(c => ({ ...c, faceDown: false }));

    while (handValue(dCards) < 17) {
      const next = nextDeck.pop();
      if (!next) break;
      dCards = [...dCards, { ...next, faceDown: false }];
    }

    return { nextDeck, dCards };
  }

  function hit() {
    const newDeck = [...deck];
    const card = newDeck.pop()!;
    const newPlayerCards = [...playerCards, card];
    setDeck(newDeck);
    setPlayerCards(newPlayerCards);

    const pSum = handValue(newPlayerCards);
    if (pSum > 21) {
      const revealed = dealerCards.map(c => ({ ...c, faceDown: false }));
      setDealerCards(revealed);
      finishGame(newPlayerCards, revealed, bet);
    } else if (pSum === 21) {
      const { nextDeck, dCards } = playDealerTurn(newDeck, dealerCards);
      setDeck(nextDeck);
      setDealerCards(dCards);
      finishGame(newPlayerCards, dCards, bet);
    }
  }

  function stand() {
    const { nextDeck, dCards } = playDealerTurn(deck, dealerCards);
    setDeck(nextDeck);
    setDealerCards(dCards);
    finishGame(playerCards, dCards, bet);
  }

  async function finishGame(pCards: Card[], dCards: Card[], betAmount: number) {
    const pSum = handValue(pCards);
    const dSum = handValue(dCards);

    let gameResult: GameResult;
    if (pSum > 21) gameResult = 'lose';
    else if (dSum > 21 || pSum > dSum) gameResult = 'win';
    else if (pSum < dSum) gameResult = 'lose';
    else gameResult = 'push';

    setResult(gameResult);
    setStatus('finished');

    try {
      if (gameResult === 'win') setProfile(await addChips(betAmount));
      else if (gameResult === 'lose') setProfile(await discardChips(betAmount));
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to update chips.');
    }
  }

  function pressHit() { hit(); }
  function pressStand() { stand(); }

  function playAgain() {
    const safeBet = Math.min(bet, profile?.chips ?? bet);
    setPlayerCards([]);
    setDealerCards([]);
    setResult(null);
    setBet(safeBet);
    setBetInput(String(safeBet));
    setStatus('betting');
  }

  if (status === 'loading') {
    return (
      <div className="bj-page">
        <p className="bj-loading">Loading profile…</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bj-page">
        <p className="bj-error-msg">{error}</p>
        <button className="btn btn--ghost" onClick={() => navigate('/')}>Back</button>
      </div>
    );
  }

  return (
    <div className="bj-page">
      <div className="bj-window">
        <div className="bj-header">
          <button className="btn btn--ghost bj-back" onClick={() => navigate('/')}>← Back</button>
          <h1 className="bj-title">Black<span>jack</span></h1>
          <div className="bj-chips">
            <span className="bj-chips__label">Chips</span>
            <span className="bj-chips__value">{profile?.chips ?? 0}</span>
          </div>
        </div>

        {status === 'betting' && (
          <div className="bj-betting">
            <p className="bj-betting__label">Place your bet</p>
            <div className="bj-bet-quick">
              {[10, 25, 50, 100].map(amt => (
                <button
                  key={amt}
                  className={`btn bj-bet-chip ${bet === amt ? 'bj-bet-chip--active' : ''}`}
                  onClick={() => selectBet(amt)}
                  disabled={amt > (profile?.chips ?? 0)}
                >
                  {amt}
                </button>
              ))}
            </div>
            <div className="bj-bet-row">
              <input
                className="bj-bet-input"
                type="number"
                min={1}
                max={profile?.chips ?? 9999}
                value={betInput}
                onChange={e => {
                  setBetInput(e.target.value);
                  const n = parseInt(e.target.value);
                  if (!isNaN(n)) setBet(n);
                }}
              />
              <button
                className="btn btn--primary-lg bj-deal-btn"
                onClick={startGame}
                disabled={!profile || bet < 1 || bet > profile.chips}
              >
                Deal
              </button>
            </div>
            {profile?.chips === 0 && (
              <p className="bj-broke">No chips left!</p>
            )}
          </div>
        )}

        {(status === 'playing' || status === 'finished') && (
          <div className="bj-table">
            <div className="bj-hand bj-hand--dealer">
              <span className="bj-hand__label">
                Dealer{status === 'finished' ? ` — ${handValue(dealerCards)}` : ''}
              </span>
              <div className="bj-cards">
                {dealerCards.map((card, i) => (
                  <div
                    key={i}
                    className={`bj-card${card.faceDown ? ' bj-card--hidden' : ''}${!card.faceDown && isRed(card.suit) ? ' bj-card--red' : ''}`}
                  >
                    {card.faceDown
                      ? <span className="bj-card__hidden">?</span>
                      : <><span className="bj-card__rank">{card.rank}</span><span className="bj-card__suit">{card.suit}</span></>}
                  </div>
                ))}
              </div>
            </div>

            {status === 'finished' && result && (
              <div className={`bj-result bj-result--${result}`}>
                {result === 'win' ? `You Win +${bet}` : result === 'lose' ? `You Lose −${bet}` : 'Push'}
              </div>
            )}

            <div className="bj-hand bj-hand--player">
              <span className="bj-hand__label">You — {handValue(playerCards)}</span>
              <div className="bj-cards">
                {playerCards.map((card, i) => (
                  <div
                    key={i}
                    className={`bj-card${isRed(card.suit) ? ' bj-card--red' : ''}`}
                  >
                    <span className="bj-card__rank">{card.rank}</span>
                    <span className="bj-card__suit">{card.suit}</span>
                  </div>
                ))}
              </div>
            </div>

            <div className="bj-actions">
              {status === 'playing' && (
                <>
                  <button className="btn btn--primary bj-action-btn" onClick={pressHit}>Hit</button>
                  <button className="btn btn--ghost bj-action-btn" onClick={pressStand}>Stand</button>
                </>
              )}
              {status === 'finished' && (
                <button className="btn btn--primary bj-action-btn" onClick={playAgain}>Play Again</button>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
