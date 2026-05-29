window.resizeImageToWebP = function (dataUrl, width, height) {
    return new Promise(function (resolve) {
        const img = new Image();
        img.onload = function () {
            const canvas = document.createElement('canvas');
            canvas.width = width;
            canvas.height = height;
            const ctx = canvas.getContext('2d');
            const scale = Math.max(width / img.width, height / img.height);
            const scaledW = img.width * scale;
            const scaledH = img.height * scale;
            const offsetX = (width - scaledW) / 2;
            const offsetY = (height - scaledH) / 2;
            ctx.drawImage(img, offsetX, offsetY, scaledW, scaledH);
            resolve(canvas.toDataURL('image/webp', 0.85));
        };
        img.src = dataUrl;
    });
};
