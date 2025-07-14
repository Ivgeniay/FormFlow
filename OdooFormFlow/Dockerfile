FROM odoo:15

USER root

RUN apt-get update && apt-get install -y \
    python3-requests \
    && rm -rf /var/lib/apt/lists/*

COPY . /mnt/extra-addons/OdooFormFlow

RUN chown -R odoo:odoo /mnt/extra-addons/OdooFormFlow

USER odoo

EXPOSE 8069