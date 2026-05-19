import { useEffect, useRef, useState } from 'react';
import { fetch as fetchMessages, send } from '../api/messageApi';
import type { Friend, Message } from '../types/friends';

const CURRENT_USER_ID = 1;

interface Props {
  friend: Friend;
  onClose: () => void;
}

export default function MessageWindow({ friend, onClose }: Props) {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const endRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    open();
  }, [friend.friendUserId]);

  useEffect(() => {
    endRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  async function open() {
    setLoading(true);
    setError(null);
    try {
      const history = await fetchMessages(CURRENT_USER_ID, friend.friendUserId);
      setMessages(history);
    } catch {
      setMessages([]);
    } finally {
      setLoading(false);
    }
  }

  async function submit() {
    if (!input.trim()) return;
    setError(null);
    try {
      const msg = await send(CURRENT_USER_ID, friend.friendUserId, input.trim());
      setMessages(prev => [...prev, msg]);
      setInput('');
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to send message.');
    }
  }

  return (
    <>
      <div className="message-panel__header">
        <span className="message-panel__title">{friend.friendUsername}</span>
        <button className="btn btn--ghost" onClick={onClose}>✕</button>
      </div>
      <div className="message-panel__history">
        {loading ? (
          <p className="friends-empty">Loading messages…</p>
        ) : messages.length === 0 ? (
          <p className="friends-empty">No messages yet.</p>
        ) : (
          messages.map(m => (
            <div
              key={m.id}
              className={`message-bubble${m.senderId === CURRENT_USER_ID ? ' message-bubble--mine' : ''}`}
            >
              <span className="message-bubble__content">{m.content}</span>
              <span className="message-bubble__time">
                {new Date(m.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
              </span>
            </div>
          ))
        )}
        <div ref={endRef} />
      </div>
      {error && <p className="bar-form__error" style={{ margin: '0 20px' }}>{error}</p>}
      <div className="message-panel__input-row">
        <input
          className="bar-form__input"
          placeholder="Type a message…"
          value={input}
          onChange={e => setInput(e.target.value)}
          onKeyDown={e => e.key === 'Enter' && submit()}
        />
        <button
          className="btn btn--primary"
          onClick={submit}
          disabled={!input.trim()}
        >
          Send
        </button>
      </div>
    </>
  );
}
