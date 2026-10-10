import { useEffect, useState } from 'react'
import './App.css'

type Product = {
    id: number
    name: string
    price: number
    categoryId: number
    category: string
}

type OrderItem = Product & {
    quantity: number
}





function App() {
    const [products, setProducts] = useState<Product[]>([])
    const [selectedCategory, setSelectedCategory] = useState('Todos')
    const [search, setSearch] = useState('')
    const [orderItems, setOrderItems] = useState<OrderItem[]>([])

    useEffect(() => {
        fetch('/api/pos/products')
            .then(response => {
                if (!response.ok) {
                    throw new Error('No se pudieron cargar los productos')
                }

                return response.json()
            })
            .then((data: Product[]) => {
                setProducts(data)
            })
            .catch(error => {
                console.error('Error cargando productos:', error)
            })
    }, [])

    const categories = [
        'Todos',
        ...Array.from(new Set(products.map(product => product.category)))
    ]

    const filteredProducts = products.filter((product) => {
        const matchesCategory =
            selectedCategory === 'Todos' ||
            product.category === selectedCategory

        const matchesSearch = product.name
            .toLowerCase()
            .includes(search.toLowerCase())

        return matchesCategory && matchesSearch
    })

    const addProduct = (product: Product) => {
        setOrderItems((currentItems) => {
            const existingItem = currentItems.find(
                (item) => item.id === product.id
            )

            if (existingItem) {
                return currentItems.map((item) =>
                    item.id === product.id
                        ? { ...item, quantity: item.quantity + 1 }
                        : item
                )
            }

            return [...currentItems, { ...product, quantity: 1 }]
        })
    }

    const clearOrder = () => {
        setOrderItems([])
    }

    const subtotal = orderItems.reduce(
        (total, item) => total + item.price * item.quantity,
        0
    )

    const serviceCharge = Math.round((subtotal * 0.10 + Number.EPSILON) * 100) / 100
    const total = Math.round((subtotal + serviceCharge + Number.EPSILON) * 100) / 100
    return (
        <div className= "pos" >
        <header className="pos-header" >
            <div>
            <h1>FoodDispatch POS </h1>
                < span > Punto de venta </span>
                    </div>

                    < div className = "pos-user" >
                        <span>Caja abierta </span>
                            < strong > Administrador </strong>
                            </div>
                            </header>

                            < main className = "pos-layout" >
                                <aside className="categories" >
                                    <h2>Categorías </h2>

    {
        categories.map((category) => (
            <button
              key= { category }
              className = {
                selectedCategory === category
                ? 'category active'
                : 'category'
              }
    onClick = {() => setSelectedCategory(category)
}
            >
{ category }
    </button>
          ))}
</aside>

    < section className = "products" >
        <div className="products-header" >
            <div>
            <h2>Productos </h2>
            < p > Selecciona un producto para agregarlo.</p>
                </div>

                < input
type = "search"
placeholder = "Buscar producto..."
value = { search }
onChange = {(event) => setSearch(event.target.value)}
            />
    </div>

    < div className = "product-grid" >
    {
        filteredProducts.map((product) => (
            <button
                key= { product.id }
                className = "product-card"
                onClick = {() => addProduct(product)}
        >
        <strong>{ product.name } </strong>
        < span > ${ product.price.toFixed(2) } </span>
            </button>
            ))}
</div>
    </section>

    < aside className = "current-order" >
        <div className="order-header" >
            <div>
            <h2>Pedido actual </h2>
                < span > Mesa 4 </span>
                    </div>

                    < button
className = "clear-button"
onClick = { clearOrder }
disabled = { orderItems.length === 0 }
    >
    Limpiar
    </button>
    </div>

    < div className = "order-items" >
        {
            orderItems.length === 0 ? (
                <p>Agrega productos al pedido.</p>
            ) : (
                    orderItems.map((item) => (
                        <div className= "order-item" key = { item.id } >
                        <div>
                        <strong>{ item.name } </strong>
                        <span>
                      { item.quantity } × ${ item.price.toFixed(2) }
                    </span>
                    </div>

                    <strong>
                    ${(item.price * item.quantity).toFixed(2)}
                        </strong>
                        </div>
                    ))
            )}
</div>

    < div className = "order-summary" >
        <div>
        <span>Subtotal </span>
        < strong > ${ subtotal.toFixed(2) } </strong>
            </div>

            < div >
            <span>Servicio 10 % </span>
                < strong > ${ serviceCharge.toFixed(2) } </strong>
                    </div>

                    < div className = "order-total" >
                        <span>Total </span>
                        < strong > ${ total.toFixed(2) } </strong>
                            </div>

                            < button
className = "charge-button"
disabled = { orderItems.length === 0 }
    >
    Cobrar ${ total.toFixed(2) }
</button>
    </div>
    </aside>
    </main>
    </div>
  )
}

export default App