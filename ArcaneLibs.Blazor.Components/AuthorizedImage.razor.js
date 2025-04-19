// export async function streamImage(stream, srcRef) {
//     console.log("streamImage start");
//     const actualStream = await stream.stream();
//     const reader = actualStream.getReader();
//
//     console.log(`StreamedImage: streamImage started for srcRef: ${srcRef}`);
//    
//     const chunks = [];
//     while (true) {
//         const {done, value} = await reader.read();
//         chunks.push(value);
//
//         if (chunks.length <= 10 || chunks.length % 10 === 0 || done) {
//             //srcRef attribute on img tag
//             let imageElement = document.querySelector(`img[srcref="${srcRef}"]`);
//             console.debug('meow', srcRef, imageElement);
//             // console.log(`StreamedImage: streamImage done: ${done}, value: ${value?.length}, chunks: ${chunks.length}`);
//             let blob = new Blob(chunks);
//             const url = window.URL.createObjectURL(blob);
//             imageElement.src = url;
//             // console.log(`StreamedImage: streamImage src: ${url} - done: ${done}`)
//
//             if (done) {
//                 console.log(`StreamedImage.razor.js: streamImage finished in chunks: ${chunks.length} -> ${url}`);
//                 return url;
//             } else {
//                 // Dispose old URLs to avoid memory leaks
//                 setTimeout(() => {
//                     window.URL.revokeObjectURL(url);
//                 }, 100);
//             }
//         }
//        
//         await new Promise(resolve => setTimeout(resolve, chunks.length * 5));
//
//         //yield to other tasks
//         // if(chunks.length > 100) {
//         //     let delay = Math.min(2000, chunks.length * 10);
//         //     console.warn(`StreamedImage: streamImage waiting ${delay} for chunks: ${chunks.length}, last read length: ${value?.length}`);
//         //     await new Promise(resolve => setTimeout(resolve, delay));
//         // }
//     }
// }

// dotnet log levels
const logLevels = {
    Trace: 0,
    Debug: 1,
    Information: 2,
    Warning: 3,
    Error: 4,
    Critical: 5,
    None: 6,
}

export async function streamImageFromUrl(url, accessToken, imageElement, logLevel) {
    // console.log("streamImageFromUrl start from", url, "for", imageElement);
    if(imageElement === null) {
        console.error("streamImageFromUrl: imageElement is null");
        return null;
    }
    const response = await fetch(url, {
        headers: {
            Authorization: `Bearer ${accessToken}`
        }
    });
    const reader = response.body.getReader();

    // console.log(`StreamedImage: streamImageFromUrl started for srcRef: ${imageElement}`);

    const chunks = [];
    while (true) {
        const {done, value} = await reader.read();
        chunks.push(value);

        if (chunks.length <= 10 || chunks.length % 10 === 0 || done) {
            // console.debug('meow', imageElement);
            // console.log(`StreamedImage: streamImageFromUrl done: ${done}, value: ${value?.length}, chunks: ${chunks.length}`);
            let blob = new Blob(chunks);
            const blobUri = window.URL.createObjectURL(blob);
            imageElement.src = blobUri;
            // console.log(`StreamedImage: streamImageFromUrl src: ${blobUri} - done: ${done}`)

            if (done) {
                if (logLevel <= logLevels.Debug)
                    console.debug(`AuthorizedImage.razor.js: streamImageFromUrl finished in ${chunks.length} chunks: ${url} -> ${blobUri}`);
                return blobUri;
            } else {
                // Dispose old URLs to avoid memory leaks
                setTimeout(() => {
                    window.URL.revokeObjectURL(blobUri);
                }, 100);
            }
        }

        await new Promise(resolve => setTimeout(resolve, chunks.length * 5));

        //yield to other tasks
        // if(chunks.length > 100) {
        //     let delay = Math.min(2000, chunks.length * 10);
        //     console.warn(`StreamedImage: streamImageFromUrl waiting ${delay} for chunks: ${chunks.length}, last read length: ${value?.length}`);
        //     await new Promise(resolve => setTimeout(resolve, delay));
        // }
    }
}