export function onLoad() {
    TinyMceInit('ProductVm_Description');
    // console.log('Loaded');
}

export function onUpdate() {
    TinyMceInit('ProductVm_Description');
    // console.log('Updated');
}

export function onDispose() {
    TinyMceDestroy('ProductVm_Description');
    // console.log('Disposed');
}
