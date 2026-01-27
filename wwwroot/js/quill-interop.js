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

        // Simple HTML to text conversion for the PDF content
        const tempDiv = document.createElement("div");
        tempDiv.innerHTML = content;
        const plainText = tempDiv.textContent || tempDiv.innerText || "";

        const splitText = doc.splitTextToSize(plainText, 170);

        // Check for page overflow
        let currentY = y;
        splitText.forEach(line => {
            if (currentY > 280) {
                doc.addPage();
                currentY = 20;
            }
            doc.text(line, 20, currentY);
            currentY += 7;
        });

        doc.save(`${(title || "journal").replace(/[^a-z0-9]/gi, '_').toLowerCase()}_${date}.pdf`);
    }
};
