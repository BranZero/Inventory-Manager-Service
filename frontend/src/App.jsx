import React, { useState, useEffect } from 'react'
import axios from 'axios'

const API = import.meta.env.VITE_API_BASE || 'https://localhost:5001'

function App(){
  const [token, setToken] = useState(localStorage.getItem('token') || '')
  const [items, setItems] = useState([])
  const [form, setForm] = useState({sku:'', name:'', qty:0})
  const [auth, setAuth] = useState({user:'', pwd:''})

  useEffect(()=>{ loadItems() }, [token])

  async function loadItems(){
    try{
      const res = await axios.get(`${API}/api/items`)
      setItems(res.data)
    }catch(e){
      console.error(e)
    }
  }

  async function register(){
    await axios.post(`${API}/api/auth/register`, { username: auth.user, password: auth.pwd })
    alert('registered; now login')
  }

  async function login(){
    const res = await axios.post(`${API}/api/auth/login`, { username: auth.user, password: auth.pwd })
    const t = res.data.token
    localStorage.setItem('token', t)
    setToken(t)
  }

  async function addItem(e){
    e.preventDefault()
    await axios.post(`${API}/api/items`, { sku: form.sku, name: form.name, quantity: Number(form.qty) }, {
      headers: { Authorization: `Bearer ${token}` }
    })
    setForm({sku:'', name:'', qty:0})
    loadItems()
  }

  return (
    <div style={{padding:20}}>
      <h2>Inventory Starter</h2>

      <section style={{marginBottom:20}}>
        <h3>Auth</h3>
        <input placeholder="user" value={auth.user} onChange={e=>setAuth(s=>({...s,user:e.target.value}))} />
        <input placeholder="pwd" type="password" value={auth.pwd} onChange={e=>setAuth(s=>({...s,pwd:e.target.value}))} />
        <button onClick={register}>Register</button>
        <button onClick={login}>Login</button>
        <div>Token: {token ? token.slice(0,40) + '...' : 'none'}</div>
      </section>

      <section>
        <h3>Items</h3>
        <form onSubmit={addItem}>
          <input placeholder="sku" value={form.sku} onChange={e=>setForm(f=>({...f,sku:e.target.value}))} required />
          <input placeholder="name" value={form.name} onChange={e=>setForm(f=>({...f,name:e.target.value}))} required />
          <input placeholder="qty" type="number" value={form.qty} onChange={e=>setForm(f=>({...f,qty:e.target.value}))} required />
          <button>Add (auth required)</button>
        </form>
        <ul>
          {items.map(it => <li key={it.id}>{it.sku} — {it.name} ({it.quantity ?? it.qty})</li>)}
        </ul>
      </section>
    </div>
  )
}

export default App