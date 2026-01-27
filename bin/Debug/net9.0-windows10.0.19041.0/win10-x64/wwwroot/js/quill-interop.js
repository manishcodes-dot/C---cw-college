window.quillInterop = {
    initialize: function (editorElement, placeholder) {
        var quill = new Quill(editorElement, {
            modules: {
                toolbar: [
                    [{ 'header': [1, 2, false] }],
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
    },
    exportToPdf: function (title, content, date, mood, category, tags) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();

        let y = 20;
        doc.setFontSize(22);
        doc.setFont(undefined, 'bold');
        doc.text(title || "Untitled", 20, y);

        y += 10;
        doc.setFontSize(10);
        doc.setFont(undefined, 'normal');
        doc.setTextColor(100, 100, 100);
        doc.text(`Date: ${date}`, 20, y);

        y += 6;
        doc.text(`Mood: ${mood} | Category: ${category}`, 20, y);

        if (tags && tags.length > 0) {
            y += 6;
            doc.text(`Tags: ${tags.join(', ')}`, 20, y);
        }

        y += 15;
        doc.setTextColor(0, 0, 0);
        doc.setFontSize(12);

        // Process content nodes to preserve some structure (like headers)
        const tempDiv = document.createElement("div");
        tempDiv.innerHTML = content;

        let currentY = y;
        Array.from(tempDiv.childNodes).forEach(node => {
            let text = "";
            let fontSize = 12;
            let fontStyle = 'normal';
            let spacing = 7;

            if (node.nodeType === 3) { // Text node
                text = node.textContent.trim();
            } else if (node.nodeType === 1) { // Element
                const tag = node.tagName.toLowerCase();
                text = node.innerText.trim();
                if (tag === 'h1') {
                    fontSize = 18;
                    fontStyle = 'bold';
                    spacing = 10;
                    currentY += 2;
                } else if (tag === 'h2') {
                    fontSize = 15;
                    fontStyle = 'bold';
                    spacing = 8;
                    currentY += 1;
                }
            }

            if (text) {
                doc.setFontSize(fontSize);
                doc.setFont(undefined, fontStyle);
                const lines = doc.splitTextToSize(text, 170);
                lines.forEach(line => {
                    if (currentY > 280) {
                        doc.addPage();
                        currentY = 20;
                    }
                    doc.text(line, 20, currentY);
                    currentY += spacing;
                });
                currentY += 2; // Extra space between paragraphs/headers
            }
        });

        doc.save(`${(title || "journal").replace(/[^a-z0-9]/gi, '_').toLowerCase()}_${date}.pdf`);
    }
};
