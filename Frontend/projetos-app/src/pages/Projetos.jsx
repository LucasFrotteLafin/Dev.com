import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import Nav from '../components/Nav'
import { getProjects, getSession } from '../store'

const CATEGORIES = ['Todas', 'E-commerce', 'Web App', 'Landing Page', 'Dashboard', 'Sistema', 'Integração', 'Mobile']

export default function Projetos() {
  const [search, setSearch] = useState('')
  const [category, setCategory] = useState('Todas')
  const navigate = useNavigate()
  const session = getSession()

  useEffect(() => {
    if (!session) window.location.replace('../auth.html')
  }, [])

  if (!session) return null

  const allOpen = getProjects().filter(p => p.status === 'open')

  const filtered = allOpen.filter(p => {
    const matchSearch = !search || p.title.toLowerCase().includes(search.toLowerCase()) || p.description.toLowerCase().includes(search.toLowerCase())
    const matchCat = category === 'Todas' || p.category === category
    return matchSearch && matchCat
  })

  return (
    <div>
      <Nav />

      <div style={{ padding: '32px', background: 'white', borderBottom: '1px solid #ddd', textAlign: 'center' }}>
        <h1 style={{ fontSize: '1.5rem', marginBottom: 4 }}>Projetos disponíveis</h1>
        <p style={{ color: '#777', fontSize: '0.9rem' }}>Encontre projetos em aberto que combinam com suas habilidades</p>
      </div>

      <div style={{ padding: '20px 32px', background: '#f4f4f4', borderBottom: '1px solid #ddd', display: 'flex', gap: 12, flexWrap: 'wrap', justifyContent: 'center', alignItems: 'center' }}>
        <input
          style={{ padding: '10px 14px', border: '1px solid #ddd', borderRadius: 6, fontSize: '0.9rem', outline: 'none', width: '100%', maxWidth: 400 }}
          placeholder="Buscar projetos..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap', justifyContent: 'center' }}>
          {CATEGORIES.map(cat => (
            <button
              key={cat}
              onClick={() => setCategory(cat)}
              style={{
                padding: '6px 14px', borderRadius: 20, fontSize: '0.8rem', cursor: 'pointer', fontWeight: category === cat ? 'bold' : 'normal',
                background: category === cat ? '#6c63ff' : 'white',
                color: category === cat ? 'white' : '#555',
                border: category === cat ? '2px solid #6c63ff' : '2px solid #ddd',
              }}
            >
              {cat}
            </button>
          ))}
        </div>
      </div>

      <div style={{ maxWidth: 860, margin: '0 auto', padding: '24px 32px' }}>
        <p style={{ fontSize: '0.85rem', color: '#777', marginBottom: 16 }}>{filtered.length} projeto(s) em aberto</p>

        {filtered.length === 0 && (
          <div style={{ textAlign: 'center', color: '#999', padding: '60px 0' }}>🔍 Nenhum projeto encontrado.</div>
        )}

        {filtered.map(p => (
          <div
            key={p.id}
            onClick={() => navigate(`/projeto/${p.id}`)}
            style={{ background: 'white', border: '1px solid #ddd', borderRadius: 8, padding: '20px 24px', marginBottom: 16, cursor: 'pointer' }}
            onMouseEnter={e => e.currentTarget.style.borderColor = '#6c63ff'}
            onMouseLeave={e => e.currentTarget.style.borderColor = '#ddd'}
          >
            <span className="badge badge-green">Aberto</span>
            <h3 style={{ fontSize: '1rem', margin: '8px 0 6px' }}>{p.title}</h3>
            <p style={{ fontSize: '0.85rem', color: '#777', lineHeight: 1.5, marginBottom: 12 }}>{p.description}</p>
            <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap', marginBottom: 12 }}>
              {p.tags.map(t => <span key={t} className="tag">{t}</span>)}
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span style={{ fontWeight: 'bold', color: '#2e7d52' }}>{p.budget}</span>
              <div style={{ fontSize: '0.78rem', color: '#999', display: 'flex', gap: 14 }}>
                <span>⏱ {p.deadline}</span>
                <span>📨 {p.proposals || 0} propostas</span>
                <span>👤 {p.client}</span>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
