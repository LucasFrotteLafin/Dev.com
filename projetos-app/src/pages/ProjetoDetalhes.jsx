import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import Nav from '../components/Nav'
import ProposalModal from '../components/ProposalModal'
import { getProjectById, getSession, getProposalsByProject, acceptProposal, seed } from '../store'

const REQUIREMENTS = [
  'Design responsivo para mobile e desktop',
  'Integração com sistema de pagamentos',
  'Painel administrativo completo',
  'Código limpo e documentado',
  'Entrega com testes funcionais',
]

function statusLabel(s) { return s === 'open' ? 'Aberto' : s === 'in_progress' ? 'Em andamento' : 'Concluído' }
function statusClass(s) { return s === 'open' ? 'badge-green' : s === 'in_progress' ? 'badge-yellow' : 'badge-purple' }

export default function ProjetoDetalhes() {
  const { id } = useParams()
  const navigate = useNavigate()
  const session = getSession()

  // garante que os projetos mock estão no localStorage mesmo em acesso direto pela URL
  seed()

  const [project, setProject] = useState(() => getProjectById(id))
  const [proposals, setProposals] = useState(() => getProposalsByProject(id))
  const [showModal, setShowModal] = useState(false)

  if (!project) return (
    <div>
      <Nav />
      <p style={{ padding: 32, color: '#999' }}>Projeto não encontrado.</p>
    </div>
  )

  const isOwner = session?.email === project.ownerEmail
  const isDev = session?.role === 'dev'
  const isClient = session?.role === 'client'
  const alreadySent = isDev && proposals.some(p => p.devEmail === session.email)

  function refresh() {
    setProject(getProjectById(id))
    setProposals(getProposalsByProject(id))
    setShowModal(false)
  }

  function handleAccept(proposalId) {
    if (!confirm('Aceitar esta proposta?')) return
    acceptProposal(proposalId)
    refresh()
  }

  return (
    <div>
      <Nav />

      {showModal && <ProposalModal project={project} onClose={() => setShowModal(false)} onSent={refresh} />}

      <div style={{ padding: '16px 32px', background: 'white', borderBottom: '1px solid #ddd' }}>
        <button onClick={() => navigate('/')} style={{ background: 'none', border: 'none', color: '#6c63ff', cursor: 'pointer', fontSize: '0.9rem' }}>
          ← Voltar para projetos
        </button>
      </div>

      <div style={{ display: 'flex', gap: 32, padding: 32, alignItems: 'flex-start' }}>

        {/* MAIN */}
        <div style={{ flex: 1 }}>
          <div style={{ display: 'flex', gap: 8, alignItems: 'center', marginBottom: 8 }}>
            <span className={`badge ${statusClass(project.status)}`}>{statusLabel(project.status)}</span>
            <span className="tag">{project.category}</span>
          </div>
          <h1 style={{ fontSize: '1.5rem', marginBottom: 8 }}>{project.title}</h1>
          <div style={{ display: 'flex', gap: 20, fontSize: '0.85rem', color: '#777', marginBottom: 24, flexWrap: 'wrap' }}>
            <span>👤 {project.client}</span>
            <span>🕐 Publicado {project.createdAt}</span>
            <span>📨 {project.proposals || 0} proposta(s)</span>
          </div>

          <Section title="Descrição"><p style={{ fontSize: '0.88rem', color: '#555', lineHeight: 1.7 }}>{project.description}</p></Section>

          <Section title="Requisitos">
            <ul style={{ listStyle: 'none', display: 'flex', flexDirection: 'column', gap: 8 }}>
              {REQUIREMENTS.map(r => <li key={r} style={{ fontSize: '0.88rem', color: '#555' }}>✓ {r}</li>)}
            </ul>
          </Section>

          <Section title="Tecnologias desejadas">
            <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
              {project.tags.map(t => <span key={t} className="tag">{t}</span>)}
            </div>
          </Section>

          {(isOwner || (isDev && alreadySent)) && (
            <Section title={`Propostas recebidas (${proposals.length})`}>
              {proposals.length === 0
                ? <p style={{ color: '#999', fontSize: '0.88rem' }}>Nenhuma proposta ainda.</p>
                : proposals.map(p => (
                  <div key={p.id} style={{ display: 'flex', gap: 14, padding: '16px 0', borderBottom: '1px solid #eee', alignItems: 'flex-start' }}>
                    <div
                      style={{ width: 36, height: 36, background: '#6c63ff', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'white', fontWeight: 'bold', fontSize: '0.78rem', flexShrink: 0, cursor: isOwner ? 'pointer' : 'default' }}
                      onClick={() => isOwner && (window.location.href = `../dev-perfil.html?dev=${encodeURIComponent(p.devEmail)}`)}
                      title={isOwner ? 'Ver perfil' : ''}
                    >
                      {p.devName.split(' ').map(w => w[0]).slice(0, 2).join('').toUpperCase()}
                    </div>
                    <div style={{ flex: 1 }}>
                      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 4 }}>
                        {isOwner
                          ? <button onClick={() => window.location.href = `../dev-perfil.html?dev=${encodeURIComponent(p.devEmail)}`} style={{ background: 'none', border: 'none', color: '#6c63ff', cursor: 'pointer', textDecoration: 'underline', fontSize: '0.9rem', fontWeight: 'bold', padding: 0 }}>{p.devName}</button>
                          : <span style={{ fontSize: '0.9rem', fontWeight: 'bold' }}>{p.devName}</span>
                        }
                        {p.status === 'accepted' && <span style={{ background: '#d4f4e2', color: '#2e7d52', padding: '2px 8px', borderRadius: 20, fontSize: '0.72rem', fontWeight: 'bold' }}>✓ Aceita</span>}
                        {p.status === 'rejected' && <span style={{ fontSize: '0.72rem', color: '#999' }}>Recusada</span>}
                      </div>
                      <p style={{ fontSize: '0.82rem', color: '#777', lineHeight: 1.5, marginBottom: 6 }}>{p.message}</p>
                      {isOwner && project.status === 'open' && p.status === 'pending' && (
                        <button onClick={() => handleAccept(p.id)} style={{ background: '#2e7d52', color: 'white', border: 'none', padding: '6px 14px', borderRadius: 6, fontSize: '0.8rem', cursor: 'pointer' }}>
                          ✓ Aceitar proposta
                        </button>
                      )}
                    </div>
                    <div style={{ textAlign: 'right', flexShrink: 0 }}>
                      <strong style={{ display: 'block', color: '#2e7d52', fontSize: '0.95rem' }}>{p.value}</strong>
                      <span style={{ fontSize: '0.78rem', color: '#999' }}>⏱ {p.deadline}</span>
                    </div>
                  </div>
                ))
              }
            </Section>
          )}
        </div>

        {/* SIDEBAR */}
        <div style={{ width: 280, flexShrink: 0 }}>
          <SideBox title="Informações">
            <InfoLine label="Orçamento" value={<span style={{ color: '#2e7d52' }}>{project.budget}</span>} />
            <InfoLine label="Prazo" value={project.deadline} />
            <InfoLine label="Categoria" value={project.category} />
            <InfoLine label="Propostas" value={project.proposals || 0} />
            {project.acceptedAt && <InfoLine label="Aceito em" value={project.acceptedAt} />}
          </SideBox>

          <SideBox title="Cliente">
            <p style={{ fontSize: '0.9rem', fontWeight: 'bold' }}>{project.client}</p>
          </SideBox>

          {!session && (
            <button className="btn btn-purple" style={{ width: '100%', padding: 12 }} onClick={() => window.location.href = '../auth.html'}>
              Entrar para enviar proposta
            </button>
          )}
          {isOwner && <button className="btn" style={{ width: '100%', padding: 12, background: '#eee', color: '#999', cursor: 'not-allowed' }} disabled>Seu projeto</button>}
          {isClient && !isOwner && <button className="btn" style={{ width: '100%', padding: 12, background: '#eee', color: '#999', cursor: 'not-allowed' }} disabled>Apenas devs enviam propostas</button>}
          {isDev && project.status !== 'open' && <button className="btn" style={{ width: '100%', padding: 12, background: '#eee', color: '#999', cursor: 'not-allowed' }} disabled>Projeto encerrado</button>}
          {isDev && project.status === 'open' && alreadySent && <button className="btn" style={{ width: '100%', padding: 12, background: '#eee', color: '#999', cursor: 'not-allowed' }} disabled>Proposta já enviada</button>}
          {isDev && project.status === 'open' && !alreadySent && (
            <button className="btn btn-purple" style={{ width: '100%', padding: 12 }} onClick={() => setShowModal(true)}>
              Enviar proposta
            </button>
          )}

          <button className="btn btn-outline" style={{ width: '100%', padding: 12, marginTop: 10 }} onClick={() => navigate('/')}>
            Ver mais projetos
          </button>
        </div>
      </div>
    </div>
  )
}

function Section({ title, children }) {
  return (
    <div style={{ background: 'white', border: '1px solid #ddd', borderRadius: 8, padding: 24, marginBottom: 20 }}>
      <h2 style={{ fontSize: '1rem', fontWeight: 'bold', marginBottom: 12 }}>{title}</h2>
      {children}
    </div>
  )
}

function SideBox({ title, children }) {
  return (
    <div style={{ background: 'white', border: '1px solid #ddd', borderRadius: 8, padding: 20, marginBottom: 16 }}>
      <h3 style={{ fontSize: '0.8rem', fontWeight: 'bold', color: '#999', textTransform: 'uppercase', marginBottom: 14 }}>{title}</h3>
      {children}
    </div>
  )
}

function InfoLine({ label, value }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.88rem', padding: '8px 0', borderBottom: '1px solid #eee' }}>
      <span style={{ color: '#777' }}>{label}</span>
      <span style={{ fontWeight: 'bold' }}>{value}</span>
    </div>
  )
}
