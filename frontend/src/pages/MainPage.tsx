import { useNavigate } from 'react-router-dom';

export default function MainPage() {
  const navigate = useNavigate();

  return (
    <div>
      <h1>Bar System</h1>
      <button onClick={() => navigate('/bars')}>My Bars</button>
    </div>
  );
}
