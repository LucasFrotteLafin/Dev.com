import { createRoot } from 'react-dom/client'
import { HashRouter, Routes, Route } from 'react-router-dom'
import Projetos from './pages/Projetos'
import ProjetoDetalhes from './pages/ProjetoDetalhes'
import './index.css'
import { seed } from './store'

seed()

createRoot(document.getElementById('root')).render(
  <HashRouter>
    <Routes>
      <Route path="/" element={<Projetos />} />
      <Route path="/projeto/:id" element={<ProjetoDetalhes />} />
    </Routes>
  </HashRouter>
)
