import * as React from "react";
import * as ReactDOM from "react-dom/client";
import {
	createBrowserRouter,
	Outlet,
	RouterProvider,
} from "react-router-dom";
import Product from './pages/products';
import Home from './pages/home';
import FrameLayout from "./layout/frameLayout";

import "./index.css"
import Login from "./pages/login";
import Register from "./pages/register";
import Admin from "./pages/admin";
import Cart from "./pages/cart";
import Orders from "./pages/orders";
import ProductManagement from "./pages/productManagement";
import Error from "./pages/error";

const router = createBrowserRouter([
	{
		path: "/",
		element: <FrameLayout><Outlet /></FrameLayout>,
		children: [
			{
				index: true,
				element: <Home />,
			},
			{
				path: "product",
				element: <Product />,
			},
			{
				path: "login",
				element: <Login />
			},
			{
				path: "register",
				element: <Register />
			},
			{
				path: "cart",
				element: <Cart />
			},
			{
				path: "orders",
				element: <Orders />
			},
			{
				path: "product_management",
				element: <ProductManagement />
			},
			{
				path: "admin",
				element: <Admin />
			},
			{
				path: "error",
				element: <Error />
			}
		],
	},
]);

ReactDOM.createRoot(document.getElementById("root")).render(
	<React.StrictMode>
		<RouterProvider router={router} />
	</React.StrictMode>
);