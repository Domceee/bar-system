import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { openSurveyForm, deleteTasteProfile } from '../api/userApi';

const USER_ID = 1;

type PageStatus = 'loading' | 'has-profile' | 'no-profile' | 'confirm-delete' | 'error';

export default function TastePage() {
  const navigate = useNavigate();
  const [status, setStatus] = useState<PageStatus>('loading');
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    openTastePage();
  }, []);

  async function openTastePage() {
    setStatus('loading');
    try {
      const { surveyNeeded } = await openSurveyForm(USER_ID);
      setStatus(surveyNeeded ? 'no-profile' : 'has-profile');
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load profile status.');
      setStatus('error');
    }
  }

  function viewTasteProfile() {
    navigate('/taste-view');
  }

  function fillSurvey() {
    navigate('/taste-survey');
  }

  function deleteTasteProfilePress() {
    setStatus('confirm-delete');
  }

  function displayConfirmation() {
    setStatus('confirm-delete');
  }

  async function submitConfirmation() {
    try {
      await deleteTasteProfile(USER_ID);
      openTastePage();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to delete profile.');
      setStatus('error');
    }
  }

  if (status === 'loading') {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Taste <span>Profile</span></h2>
          <p>Loading…</p>
        </div>
      </div>
    );
  }

  if (status === 'error') {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Taste <span>Profile</span></h2>
          <div className="bar-form__error">{error}</div>
          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={() => navigate('/')}>Back</button>
          </div>
        </div>
      </div>
    );
  }

  if (status === 'confirm-delete') {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Delete <span>Profile</span></h2>
          <p style={{ marginBottom: '8px', color: 'var(--text-muted)' }}>
            Are you sure you want to delete your taste profile? This cannot be undone.
          </p>
          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={() => setStatus('has-profile')}>Cancel</button>
            <button className="btn btn--delete btn--action" onClick={submitConfirmation}>Delete</button>
          </div>
        </div>
      </div>
    );
  }

  if (status === 'no-profile') {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Taste <span>Profile</span></h2>
          <p style={{ marginBottom: '24px', color: 'var(--text-muted)' }}>
            You don't have a taste profile yet. Fill out a short survey to get personalised bar recommendations.
          </p>
          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={() => navigate('/')}>Back</button>
            <button className="btn btn--primary" onClick={fillSurvey}>Fill Survey</button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bar-form-page">
      <div className="bar-form">
        <h2 className="bar-form__title">Taste <span>Profile</span></h2>
        <p style={{ marginBottom: '24px', color: 'var(--text-muted)' }}>
          You have an existing taste profile.
        </p>
        <div className="taste-page__actions">
          <button className="btn btn--primary taste-page__btn" onClick={viewTasteProfile}>
            View Profile
          </button>
          <button className="btn btn--ghost taste-page__btn" onClick={fillSurvey}>
            Fill New Survey
          </button>
          <button className="btn btn--delete taste-page__btn" onClick={deleteTasteProfilePress}>
            Delete Profile
          </button>
        </div>
        <div className="bar-form__actions" style={{ marginTop: '24px' }}>
          <button className="btn btn--ghost" onClick={() => navigate('/')}>Back</button>
        </div>
      </div>
    </div>
  );
}
