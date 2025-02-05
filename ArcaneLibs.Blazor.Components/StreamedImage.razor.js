export async function streamImage(stream, srcRef) {
    console.log("streamImage start");
    const actualStream = await stream.stream();
    const reader = actualStream.getReader();

    console.log(`StreamedImage: streamImage started for srcRef: ${srcRef}`);
    
    const chunks = [];
    while (true) {
        const {done, value} = await reader.read();
        chunks.push(value);

        if (chunks.length <= 10 || chunks.length % 10 === 0 || done) {
            //srcRef attribute on img tag
            let imageElement = document.querySelector(`img[srcref="${srcRef}"]`);
            console.debug('meow', srcRef, imageElement);
            // console.log(`StreamedImage: streamImage done: ${done}, value: ${value?.length}, chunks: ${chunks.length}`);
            let blob = new Blob(chunks);
            const url = window.URL.createObjectURL(blob);
            imageElement.src = url;
            // console.log(`StreamedImage: streamImage src: ${url} - done: ${done}`)

            if (done) {
                console.log(`StreamedImage.razor.js: streamImage finished in chunks: ${chunks.length} -> ${url}`);
                return url;
            } else {
                // Dispose old URLs to avoid memory leaks
                setTimeout(() => {
                    window.URL.revokeObjectURL(url);
                }, 100);
            }
        }
        
        await new Promise(resolve => setTimeout(resolve, chunks.length * 5));

        //yield to other tasks
        // if(chunks.length > 100) {
        //     let delay = Math.min(2000, chunks.length * 10);
        //     console.warn(`StreamedImage: streamImage waiting ${delay} for chunks: ${chunks.length}, last read length: ${value?.length}`);
        //     await new Promise(resolve => setTimeout(resolve, delay));
        // }
    }
}