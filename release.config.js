module.exports={
    plugins: [
      ["@semantic-release/commit-analyzer", {
          releaseRules: [
              {"type": "major"  , "release": "major"},
              {"type": "release", "release": "major"},
          ],
          parserOpts: {
              "noteKeywords": ["BREAKING CHANGE", "BREAKING CHANGES", "BREAKING"]
          }
      }],
  
      ["@semantic-release/exec",{
          prepareCmd: `CR=docker.io \
           && docker login $CR -u $DOCKER_HUB_USER -p $DOCKER_HUB_PASSWORD \
           && docker build -f ci/Dockerfile.api          -t $CR/elders/pushnotifications.api:\${nextRelease.version}      $STAGING_PATH \
           && docker build -f ci/Dockerfile.svc      -t $CR/elders/pushnotifications.svc:\${nextRelease.version}  $STAGING_PATH \
           && docker push $CR/elders/pushnotifications.api:\${nextRelease.version} \
           && docker push $CR/elders/pushnotifications.svc:\${nextRelease.version} \
          `,
      }],
  
      "@semantic-release/release-notes-generator",
      "@semantic-release/changelog",
      "@semantic-release/git"
    ],
  
    branches: [
      'master',
      { name: 'release-*', prerelease: true },
    ],
  }
  