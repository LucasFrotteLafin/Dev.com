import { useState } from 'react'
import { sendProposal, getSession } from '../store'

export default function ProposalModal({ project, onClose, onSent }) {
  const [value, setValue] = useState('')
  const [deadline, setDeadline] = useState('')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  function handleSubmit(e) {
    e.preventDefault()
    const session = getSession()
    const res = sendProposal({
      projectId: project.id,
      devEmail: session.email,
      devName: session.name,
      value,
      deadline,
      message,
    })
    if (!res.ok) { setError(res.msg); return }
    onSent()
  }

  return (
    <div style={overlay} onClick={onClose}>
      <div style={modal} onClick={e => e.stopPropagation()}>
        <h2 style={{ fontSize: '1.1rem', marginBottom: 4 }}>Enviar proposta</h2>
        <p style={{ fontSize: '0.82rem', color: '#777', marginBottom: 20 }}>{project.title}</p>
        <form onSubmit={handleSubmit}>
          <div style={fg}>
            <label style={label}>Seu valor</label>
            <input style={input} placeholder="Ex: R$ 2.500" value={value} onChange={e => setValue(e.target.value)} required />
          </div>
          <div style={fg}>
            <label style={label}>Prazo estimado</label>
            <input style={input} placeholder="Ex: 20 dias" value={deadline} onChange={e => setDeadline(e.target.value)} required />
          </div>
          <div style={fg}>
            <label style={label}>Mensagem para o cliente</label>
            <textarea style={{ ...input, minHeight: 80, resize: 'vertical' }} placeholder="Explique sua experiência..." value={message} onChange={e => setMessage(e.target.value)} required />
          </div>
          {error && <p style={{ color: '#c0392b', fontSize: '0.82rem', marginBottom: 8 }}>{error}</p>}
          <div style={{ display: 'flex', gap: 10 }}>
            <button className="btn btn-purple" type="submit" style={{ flex: 1 }}>Enviar proposta</button>
            <button className="btn btn-outline" type="button" onClick={onClose}>Cancelar</button>
          </div>
        </form>
      </div>
    </div>
  )
}

const overlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.4)', zIndex: 100, display: 'flex', alignItems: 'center', justifyContent: 'center' }
const modal = { background: 'white', borderRadius: 10, padding: 32, width: '100%', maxWidth: 480 }
const fg = { marginBottom: 14 }
const label = { display: 'block', fontSize: '0.82rem', fontWeight: 'bold', marginBottom: 5, color: '#555' }
const input = { width: '100%', padding: '9px 12px', border: '1px solid #ddd', borderRadius: 6, fontSize: '0.9rem', outline: 'none', fontFamily: 'inherit' }
