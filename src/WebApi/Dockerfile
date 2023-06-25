ARG BUILD_IMAGE=''
ARG RUNTIME_IMAGE=''

FROM ${BUILD_IMAGE} AS builder

COPY src/ /src/
COPY ProjectConfig.xml /

ARG SkipGenerateSwagger=true
RUN dotnet restore /src/WebApi/WebApi.csproj && \
    dotnet publish /src/WebApi/WebApi.csproj -c Release -o /app --no-restore

FROM ${RUNTIME_IMAGE} AS runner
ARG UID=1000
ARG USER=webapi-user

RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib

WORKDIR /app

RUN addgroup --system "$USER" && \
    adduser --disabled-password --gecos "" --ingroup "$USER" --uid "$UID" --system "$USER" && \
    chown -R $UID:$UID /app

COPY --chown=$UID:$UID --from=builder /app .

USER $USER
EXPOSE 80
ENTRYPOINT ["dotnet", "WebApi.dll"]