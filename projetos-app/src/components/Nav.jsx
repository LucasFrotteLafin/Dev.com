import { getSession } from '../store'

export default function Nav() {
  const session = getSession()
  const base = '../'

  return (
    <nav>
      <div className="logo" onClick={() => window.location.href = base + 'index.html'}>Dev.com</div>
      <ul>
        <li><a href={base + 'index.html'}>Início</a></li>
        <li><a href={base + 'dashboard.html'}>Dashboard</a></li>
      </ul>
      {session
        ? <button className="nav-btn" onClick={() => window.location.href = base + 'dashboard.html'}>{session.name.split(' ')[0]}</button>
        : <button className="nav-btn" onClick={() => window.location.href = base + 'auth.html'}>Entrar</button>
      }
    </nav>
  )
}
