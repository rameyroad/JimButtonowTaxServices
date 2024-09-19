import React, { useEffect } from "react";

import { Block } from "../types/dynamicPage";

export interface BlockProps {
    block: Block;
}

export const ImageGalleryBlock: React.FC<BlockProps> = ({ block }) => {
    // const [images, setImages] = useState<Array<ReactImageGalleryItem>>([]);

    useEffect(() => {
        console.log("block", block);
        // if (block.items) {
        //     var imgArray = new Array<ReactImageGalleryItem>();
        //     let max = block.items?.length ?? 0;
        //     for (var i = 0; i < max; ++i) {
        //         let media = block.items[i].body?.media;
        //         if (media) {
        //             let item = {
        //                 original: media?.publicUrl,
        //                 thumbnail: media?.publicUrl,
        //             };
        //             imgArray.push(item);
        //         }
        //     }
        //     setImages(imgArray);
        // }
    }, []);

    return (
        <div className="block image-gallery-block">
            <div className="container">
                {/* <ImageGallery items={images} showFullscreenButton={false} autoPlay={true} /> */}
            </div>
        </div>
    );
};
