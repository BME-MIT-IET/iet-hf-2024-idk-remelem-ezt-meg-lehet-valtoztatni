import Header from "./header";

const FrameLayout = ({ children }) => {
	return (
		<>
			<header><Header /></header>
			<main>{children}</main>
		</>
	);
};

export default FrameLayout;