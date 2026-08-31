console.log("local-time.js executado");

document.querySelectorAll(".local-date")
    .forEach(element => {

        console.log("Encontrei:", element);

        const value = element.dataset.date;
        const format = element.dataset.format;

        if (!value) {
            return;
        }

        const date = new Date(value);

        if (isNaN(date.getTime())) {
            element.textContent = "Data inválida";
            return;
        }

        const dateText =
            date.toLocaleDateString("pt-PT", {
                day: "2-digit",
                month: "2-digit",
                year: "numeric"
            });

        const timeText =
            date.toLocaleTimeString("pt-PT", {
                hour: "2-digit",
                minute: "2-digit",
                hour12: false
            });

        console.log("Data:", dateText);
        console.log("Hora:", timeText);
        console.log("Formato:", format);

        if (format === "long") {
            element.textContent =
                `${dateText} ${timeText}`;

            return;
        }

        if (format === "long-pt") {
            element.textContent =
                `${dateText} às ${timeText}`;

            return;
        }

        if (format === "long-comma") {
            return;
        }
       

        if (format === "time") {
            element.textContent = timeText;
            return;
        }

        if (format === "date") {
            element.textContent = dateText;
            return;
        }

        element.innerHTML = `
            <div>${dateText}</div>
            <small class="text-muted">${timeText}</small>
        `;
    });