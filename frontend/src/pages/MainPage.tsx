import { useNavigate } from 'react-router-dom';

export default function MainPage() {
  const navigate = useNavigate();

  return (
    <div className="main-page">
      <div style={{ textAlign: 'center', display: 'flex', flexDirection: 'column', gap: '12px' }}>
        <h1 className="main-page__title">
          Bar<span>System</span>
        </h1>
        <p className="main-page__sub">venue management</p>
      </div>
      <button className="btn--primary-lg" onClick={() => navigate('/bars')}>
        Open Bars
      </button>
    </div>
  );
}
