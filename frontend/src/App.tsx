import { BrowserRouter, Routes, Route } from 'react-router-dom';
import MainPage from './pages/MainPage';
import BarList from './pages/BarList';
import BlackjackGame from './pages/BlackjackGame';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainPage />} />
        <Route path="/bars" element={<BarList />} />
        <Route path="/blackjack" element={<BlackjackGame />} />
      </Routes>
    </BrowserRouter>
  );
}
