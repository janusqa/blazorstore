export function onLoad() {
    TinyMceInit('ProductDto_Description');
    console.log('Loaded');
}

export function onUpdate() {
    TinyMceInit('ProductDto_Description');
    console.log('Updated');
}

export function onDispose() {
    // console.log('Disposed');
}
