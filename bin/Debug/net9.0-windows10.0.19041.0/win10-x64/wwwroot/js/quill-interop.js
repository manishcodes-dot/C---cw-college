window.quillInterop = {
    initialize: function (editorElement, placeholder) {
        var quill = new Quill(editorElement, {
            modules: {
                toolbar: [
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                    ['clean']
                ]
            },
            placeholder: placeholder,
            theme: 'snow'
        });

        // Store quill instance on the element for retrieval
        editorElement.quill = quill;
    },
    getContent: function (editorElement) {
        return editorElement.quill ? editorElement.quill.root.innerHTML : "";
    },
    setContent: function (editorElement, content) {
        if (editorElement.quill) {
            editorElement.quill.root.innerHTML = content || "";
        }
    }
};
