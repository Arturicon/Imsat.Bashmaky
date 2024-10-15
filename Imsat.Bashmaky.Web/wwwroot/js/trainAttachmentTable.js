(function () {
    if (window.customElements.get("train-attachment-table") !== undefined) return;
    const template = document.createElement("template");
    template.innerHTML = `
    <div>
        <table class="table">
            <thead>
                <tr>
                  <th scope="col">Выбрать</th>
                  <th scope="col">ID</th>
                  <th scope="col">Дата закрепления</th>
                  <th scope="col">Закрепивший</th>
                  <th scope="col">Башмаки</th>
                  <th scope="col">Дата снятия</th>
                  <th scope="col">Снявший</th>
                </tr>
             </thead>
             <tbody id="attachTable">
              </tbody>
        </table>
        <button id="createBtn">Create</button>
        <button id="deleteBtn">Delete</button>
        <button id="saveBtn">Save</button>
    </div>
    `;
    class TrainAttachmentTable extends HTMLElement {
        constructor() {
            super();
            this.append(template.content.cloneNode(true));
        }


        async connectedCallback() {
            this.tableBody = this.querySelector('[id="attachTable"]');
            const createBtn = this.querySelector('[id="createBtn"]');
            createBtn.addEventListener('click', () => this.createAttachment());
            const deleteBtn = this.querySelector('[id="deleteBtn"]');
            deleteBtn.addEventListener('click', () => this.deleteAttachments());
            const saveBtn = this.querySelector('[id="saveBtn"]');
            saveBtn.addEventListener('click', () => this.saveBtn());
            this.getAttachments();
        }



        async getAttachments() {
            const resp = await fetch(`/api/entity/get-attachments`);
            if (!resp.ok)
                return;
            const data = await resp.json();
            data.forEach(async (item) => {
                await this.addRow(item);
            });

        }

        async createAttachment() {
            const resp = await fetch(`/api/entity/create-attachment`, {
                method: 'POST'
            });
            if (!resp.ok)
                return;
            const data = await resp.json();
            await this.addRow(data);
        }



        async deleteAttachments() {
            const checkboxes = this.querySelectorAll('.select-row');
            const rows = this.tableBody.querySelectorAll('tr');
            const selectedRows = [];
            checkboxes.forEach((checkbox, index) => {
                if (checkbox.checked) {
                    selectedRows.push(rows[index].children[1].innerText);
                }
            });

            const resp = await fetch(`/api/entity/delete-attachments`, {
                method: 'POST',
                headers:
                {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(selectedRows)
            });
            if (!resp.ok) {
                console.log('error!')
                return;
            }
            window.location.reload();
        }


        async saveBtn() {
            let attachaments =[]

            $('#attachTable tr').each(function () {
                let attachValue = $(this).find('td[name="attach"]').text().trim(); 
                let optionsText = [];
                $(this).find("li[class='select2-selection__choice']").each((function () {
                    optionsText.push($(this).text().trim().replace('×', ''));
                }));
                let bashmaks = optionsText.map(function (x) {
                    return {mac:x}
                });
                attachaments.push({ id: attachValue, bashmakies: bashmaks });
            });

             await fetch(`/api/entity/save-attachments`, {
                method: 'POST',
                headers:
                {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(attachaments)
            });
        }

        async addRow(data) {
            const requestedStatuses = ["Raised", "PutOnRail"]
            const resp = await fetch(`/api/entity/get-bashmaks`, {
                method: 'POST',
                headers:
                {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestedStatuses)
            });
            if (!resp.ok)
                return;
            const freeBashmaki = await resp.json()

            const row = this.createAttachmentRow(data.id, data.attachingTimeUtc, data.attachingFitter, freeBashmaki, data.detachingTimeUtc, data.detachingFitter, data.bashmakies);
            this.tableBody.insertRow().innerHTML = row;
            $(`#bashmaki${data.id}`).select2({
                placeholder: "Выберите башмаки",
                allowClear: true
            });
        }

        createAttachmentRow(id, attDate, attachman, freeBashmaki, detDate, detachman, busyBashmaks) {
            let options = '';
            const busyBashmaksIds = busyBashmaks.map(function (x) { return x.mac }); //!!!!!!!!!!!!!
            freeBashmaki.forEach((item, index) => {
                if (busyBashmaksIds.indexOf(item.mac) != -1)
                    options += `<option value="${index}" selected>${item.mac}</option>`;
                else
                    options += `<option value="${index}">${item.mac}</option>`;
            });
            return `
                      <td><input type="checkbox" class="select-row"></td>
                      <td name="attach">${id}</th>
                      <td>${attDate}</td>
                      <td>${attachman}</td>
                      <td>
                        <select name="bashmaksOpts" id="bashmaki${id}" class="form-select" multiple aria-label="Multiple select example">
                            ${options}
                        </select>
                      </td>
                      <td>${detDate}</td>
                      <td>${detachman}</td>
                    `;
        }
    }

    customElements.define("train-attachment-table", TrainAttachmentTable)
})();


