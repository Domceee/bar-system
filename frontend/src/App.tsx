import { BrowserRouter, Routes, Route } from 'react-router-dom';
import MainPage from './pages/MainPage';
import BarList from './pages/BarList';
import ReservationList from './pages/ReservationList';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainPage />} />
        <Route path="/bars" element={<BarList />} />
        <Route path="/reservations" element={<ReservationList />} />
      </Routes>
    </BrowserRouter>
  );
}
