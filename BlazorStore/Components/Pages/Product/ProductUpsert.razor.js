export function onLoad() {
    blazorInterop.TinyMceInit('ProductVm_Description');
    // console.log('Loaded');
}

export function onUpdate() {
    blazorInterop.TinyMceInit('ProductVm_Description');
    // console.log('Updated');
}

export function onDispose() {
    blazorInterop.TinyMceDestroy('ProductVm_Description');
    // console.log('Disposed');
}
