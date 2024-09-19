import React, { useEffect } from "react";
import { DynamicPage } from "../types/dynamicPage";

interface Props {
    page: DynamicPage;
}

export const Hero = ({ page }: Props): JSX.Element => {
    useEffect(() => {
        // const jarallaxInit = async () => {
        //   const jarallaxElems = document.querySelectorAll('.jarallax');
        //   if (!jarallaxElems || (jarallaxElems && jarallaxElems.length === 0)) {
        //     return;
        //   }
        //   const { jarallax } = await import('jarallax');
        //   jarallax(jarallaxElems, { speed: 0.2 });
        // };
        // jarallaxInit();
    });

    return (
        <div
            style={{
                position: "relative",
                height: "300",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                paddingTop: "100px",
                backgroundPosition: "bottom right",
                backgroundSize: "cover",
                backgroundImage: `url('${page?.primaryImage?.media?.publicUrl}')`,
                backgroundRepeat: "no-repeat",
            }}
        >
            <div
                style={{
                    position: "absolute",
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    width: 1,
                    height: 1,
                    background: "#161c2d04",
                    zIndex: 1,
                }}
            />
            <div
                className="container"
                style={{
                    position: "relative",
                    zIndex: 2,
                }}
            >
                {page.showTitle && (
                    <h2
                        style={{
                            fontWeight: 900,
                            color: "common.white",
                            textTransform: "uppercase",
                        }}
                    >
                        {page?.title}
                    </h2>
                )}
                {page.showExcerpt && page?.excerpt && (
                    <div
                        style={{
                            borderRadius: 1,
                            backgroundColor: "rgba(255,255,255,0.75)",
                            width: "70%",
                            padding: "5px 10px",
                            margin: "20px 0",
                        }}
                    >
                        <p
                            style={{
                                color: "#415058",
                                fontSize: 16,
                                fontWeight: 600,
                            }}
                        >
                            {page.excerpt}
                        </p>
                    </div>
                )}
            </div>
        </div>
    );
};
