import { BrowserRouter, Routes, Route } from 'react-router-dom';
import MainPage from './pages/MainPage';
import BarList from './pages/BarList';
<<<<<<< HEAD
import BlackjackGame from './pages/BlackjackGame';
=======
import ReservationList from './pages/ReservationList';
import TasteSurvey from './pages/TasteSurvey';
import BarRecPage from './pages/BarRecPage';
import CocktailRecipeList from './pages/CocktailRecipeList';
>>>>>>> 4e2a57b0275e3df79edbdf0ef911869d606f1b66

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainPage />} />
        <Route path="/bars" element={<BarList />} />
<<<<<<< HEAD
        <Route path="/blackjack" element={<BlackjackGame />} />
=======
        <Route path="/reservations" element={<ReservationList />} />
        <Route path="/taste-survey" element={<TasteSurvey />} />
        <Route path="/bar-recommendation" element={<BarRecPage />} />
        <Route path="/recipes" element={<CocktailRecipeList />} />
>>>>>>> 4e2a57b0275e3df79edbdf0ef911869d606f1b66
      </Routes>
    </BrowserRouter>
  );
}
