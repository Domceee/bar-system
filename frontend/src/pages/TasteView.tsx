import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchTasteProfile } from '../api/userApi';
import type { TasteProfileResponse } from '../types/taste';

const USER_ID = 1;

const QUESTION_LABELS: Record<string, string> = {
  budget: "Drink budget per drink",
  flavor_balance: "Preferred flavor balance",
  drink_strength: "Preferred drink strength",
  drink_type: "Go-to drink",
  flavor_profile: "Favourite flavor profile",
  bar_distance: "Willing to travel",
  bar_rating: "Minimum bar rating",
  bar_design: "Preferred bar interior",
};

export default function TasteView() {
  const navigate = useNavigate();
  const [profile, setProfile] = useState<TasteProfileResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchTasteProfile(USER_ID)
      .then(setProfile)
      .catch(e => setError(e instanceof Error ? e.message : 'Failed to load profile.'));
  }, []);

  if (error) {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Taste <span>Profile</span></h2>
          <div className="bar-form__error">{error}</div>
          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={() => navigate('/taste')}>Back</button>
          </div>
        </div>
      </div>
    );
  }

  if (!profile) {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Taste <span>Profile</span></h2>
          <p>Loading…</p>
        </div>
      </div>
    );
  }

  return (
    <div className="bar-form-page">
      <div className="bar-form">
        <h2 className="bar-form__title">Taste <span>Profile</span></h2>
        <p style={{ fontSize: '11px', color: 'var(--text-muted)', marginBottom: '20px', letterSpacing: '0.5px' }}>
          Created {new Date(profile.createdAt).toLocaleDateString()}
        </p>
        <ul className="taste-survey__review">
          {profile.answers.map(a => (
            <li key={a.questionKey}>
              <strong>{QUESTION_LABELS[a.questionKey] ?? a.questionKey}</strong>
              <div className="taste-survey__review-answer">{a.answer}</div>
            </li>
          ))}
        </ul>
        <div className="bar-form__actions" style={{ marginTop: '24px' }}>
          <button className="btn btn--ghost" onClick={() => navigate('/taste')}>Back</button>
        </div>
      </div>
    </div>
  );
}
