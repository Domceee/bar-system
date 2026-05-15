import { BrowserRouter, Routes, Route } from 'react-router-dom';
import MainPage from './pages/MainPage';
import BarList from './pages/BarList';
import ReservationList from './pages/ReservationList';
import TasteSurvey from './pages/TasteSurvey';
import BarRecPage from './pages/BarRecPage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainPage />} />
        <Route path="/bars" element={<BarList />} />
        <Route path="/reservations" element={<ReservationList />} />
        <Route path="/taste-survey" element={<TasteSurvey />} />
        <Route path="/bar-recommendation" element={<BarRecPage />} />
      </Routes>
    </BrowserRouter>
  );
}
