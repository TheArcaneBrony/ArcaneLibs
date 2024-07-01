export async function streamImage(stream, imageElement) {
    const actualStream = await stream.stream();
    const reader = actualStream.getReader();

    const chunks = [];
    let blob = null;
    while (true) {
        const {done, value} = await reader.read();
        console.log(`streamImage done: ${done}, value: ${value?.length}, chunks: ${chunks.length}`);
        chunks.push(value);

        if (blob !== null) {
            window.URL.revokeObjectURL(blob);
        }
        blob = new Blob(chunks, {type: 'image/jpeg'});
        const url = window.URL.createObjectURL(blob);
        imageElement.src = url;
        setTimeout(() => {
            window.URL.revokeObjectURL(url);
        }, 100);
        if (done) {
            break;
        }
    }
}